using Emgu.CV.Structure;
using Emgu.CV;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using System.IO;


namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private ISaleService _saleService = new SaleService();
        private readonly ICameraService _cameraService = new CameraService();
        private IImageService _imageService = new ImageService();
        private CustomerService customerService = new();
        private CameraService cameraService = new();
        private bool isExpanded = false;
        public Account CurrentAccount { get; set; } = null;
        private BrokerWindow broker;
        private List<Sale> _sessionSaleList { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            broker = new BrokerWindow();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            
        }

        private void OnSalesDataUpdated(object sender, List<Sale> updatedSales)
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine(updatedSales);
                SalesDataGrid.ItemsSource = null;
                SalesDataGrid.ItemsSource = updatedSales;
            });
        }


        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                // Check if the message starts with "Can_ta" or "Can_tieu_ly"
                string[] parts = data.Split(':');
                if (parts.Length == 3 && float.TryParse(parts[2], out float currentValue))
                {
                    string rfidString = parts[1];
                    string messageType = parts[0] == "Can_ta" ? "Weight" : parts[0] == "Can_tieu_ly" ? "Density" : null;
                    Debug.WriteLine(data);
                    Debug.WriteLine(messageType);

                    if (messageType != null)
                    {
                        Camera newestCamera = await cameraService.GetNewestCameraAsync();
                        DateTime currentTime = DateTime.Now;
                        Sale sale = await _saleService.GetSaleByRFIDCodeWithoutDensity(rfidString);
                        Debug.WriteLine("Khach hang"+ sale);

                        if (sale == null && messageType == "Weight")
                        {
                            Customer customer = await customerService.GetCustomerByRFIDCodeAsync(rfidString);
                            if (customer == null)
                            {
                                MessageBox.Show("Không thể tìm thấy khách hàng với rfid này", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            sale = new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidString,
                                ProductWeight = currentValue,
                                LastEditedTime = currentTime,
                                ProductDensity = 0,
                                Status = 1,
                                CustomerName = customer.CustomerName
                            };
                            await _saleService.CreateSaleAsync(sale);

                            // Capture image from the camera
                            string imagePath = CaptureImageFromCamera(newestCamera, 1);
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                Repository.Models.Image image = new Repository.Models.Image
                                {
                                    ImageId = Guid.NewGuid(),
                                    ImageType = 1,
                                    ImagePath = imagePath,
                                    CreatedDate = currentTime,
                                    SaleId = sale.SaleId
                                };
                                await _imageService.AddImageAsync(image);
                            }

                            // Add the new sale to _sessionSaleList
                            _sessionSaleList.Add(sale);
                        }
                        else if (sale != null)
                        {
                            // Handle updates based on message type (Weight or Density)
                            if (messageType == "Weight" && sale.ProductDensity == 0)
                            {
                                sale.ProductWeight += currentValue;
                                await _saleService.UpdateSaleAsync(sale);

                                // Capture image for weight update
                                string imagePath = CaptureImageFromCamera(newestCamera, 1);
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    Repository.Models.Image image = new Repository.Models.Image
                                    {
                                        ImageId = Guid.NewGuid(),
                                        ImageType = 1,
                                        ImagePath = imagePath,
                                        CreatedDate = currentTime,
                                        SaleId = sale.SaleId
                                    };
                                    await _imageService.AddImageAsync(image);
                                }

                                // Update the sale in _sessionSaleList
                                Sale existingSale = _sessionSaleList.FirstOrDefault(s => s.SaleId == sale.SaleId);
                                if (existingSale != null)
                                {
                                    existingSale.ProductWeight = sale.ProductWeight;
                                    existingSale.LastEditedTime = currentTime;
                                }
                            }
                            else if (messageType == "Density" && sale.ProductWeight.HasValue && sale.ProductDensity == 0)
                            {
                                Debug.WriteLine($"Updating density for Sale: {sale.SaleId}, Weight: {sale.ProductWeight}, Current Density: {sale.ProductDensity}, New Density: {currentValue}");
                                if (currentValue > 100)
                                {
                                    MessageBox.Show("Tỉ trọng không thể vượt quá 100 %", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                sale.ProductDensity = currentValue;
                                await _saleService.UpdateSaleAsync(sale);

                                // Capture image for density update
                                string imagePath = CaptureImageFromCamera(newestCamera, 2);
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    Repository.Models.Image image = new Repository.Models.Image
                                    {
                                        ImageId = Guid.NewGuid(),
                                        ImageType = 2,
                                        ImagePath = imagePath,
                                        CreatedDate = currentTime,
                                        SaleId = sale.SaleId
                                    };
                                    await _imageService.AddImageAsync(image);
                                }

                                // Update the sale in _sessionSaleList
                                Sale existingSale = _sessionSaleList.FirstOrDefault(s => s.SaleId == sale.SaleId);
                                if (existingSale != null)
                                {
                                    existingSale.ProductDensity = sale.ProductDensity;
                                    existingSale.LastEditedTime = currentTime;
                                }
                            }
                        }

                        // Refresh DataGrid
                        SalesDataGrid.Dispatcher.Invoke(() =>
                        {
                            SalesDataGrid.ItemsSource = null;
                            SalesDataGrid.ItemsSource = _sessionSaleList;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Unknown message format.");
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to parse message data.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        private string CaptureImageFromCamera(Camera camera, int cameraIndex)
        {
            string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"{Guid.NewGuid()}_Camera{cameraIndex}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (string.IsNullOrEmpty(cameraUrl))
                {
                    throw new Exception($"URL của Camera {cameraIndex} không hợp lệ.");
                }

                using (var capture = new VideoCapture(cameraUrl))
                {
                    if (!capture.IsOpened)
                    {
                        throw new Exception($"Không thể mở Camera {cameraIndex}.");
                    }

                    using (var frame = new Mat())
                    {
                        capture.Read(frame);
                        if (frame.IsEmpty)
                        {
                            throw new Exception($"Không thể chụp ảnh từ Camera {cameraIndex}.");
                        }

                        // Chuyển đổi frame sang Image<Bgr, byte>
                        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();

                        // Chuyển đổi Image<Bgr, byte> sang Bitmap
                        Bitmap bitmap = image.ToBitmap();

                        // Lưu hình ảnh vào đĩa
                        bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh từ Camera {cameraIndex}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return localFilePath.ToString();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Hành động này sẽ đóng ứng dụng, bạn chắc chứ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }

        }


        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.CurrentAccount = CurrentAccount;
            Close();
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            broker.Show();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đang ở cửa sổ hiển thị!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(isExpanded == true)
            {
                ExpandButton.Visibility = Visibility.Hidden;
                CloseButton.Visibility = Visibility.Visible;
            }
            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("CreateRFID");
                await _mqttClientService.SubscribeAsync("Can_ta");
                await _mqttClientService.SubscribeAsync("Can_tieu_ly");
                _mqttClientService.MessageReceived += OnMqttMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
            }
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }


        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
            }
            int choice = 0;
            Sale scannedSale = new();
            if (SalesDataGrid.SelectedItem is Sale selectedSale)
            {
                scannedSale = selectedSale;
                if (selectedSale.ProductWeight == null && selectedSale.ProductDensity == null)
                {
                    choice = 1;
                }
                else if (selectedSale.ProductDensity == null && selectedSale.ProductWeight != null)
                {
                    choice = 2;
                }
                else if (selectedSale.ProductWeight == null && selectedSale.ProductDensity != null)
                {
                    choice = 3;
                }
            }
            switch (choice)
            {
                case 1:
                    CreateRFID(scannedSale);
                    break;
                case 2:
                    await AddWeight(scannedSale);
                    break;
                case 3:
                    await AddDensity(scannedSale);
                    break;
                default:
                    break;
            }
        }
        private void CreateRFID(Sale sale)
        {
            RFIDManagementWindow window = new RFIDManagementWindow(sale.RFIDCode);
            window.ShowDialog();
        }

        private async Task AddWeight(Sale sale)
        {
            MessageBox.Show("Dữ liệu cân tạ cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }
        private async Task AddDensity(Sale sale)
        {
            MessageBox.Show("Dữ liệu cân tiểu li cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    this.Hide();
        //    e.Cancel = true;
        //}

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow viewWindow = new();
            viewWindow._sessionSaleList = this._sessionSaleList;
            viewWindow.CurrentAccount = this.CurrentAccount;

            viewWindow.WindowState = WindowState.Maximized;
            viewWindow.WindowStyle = WindowStyle.None;
            viewWindow.ResizeMode = ResizeMode.NoResize; 
            viewWindow.isExpanded = true;
            viewWindow.Show();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}





