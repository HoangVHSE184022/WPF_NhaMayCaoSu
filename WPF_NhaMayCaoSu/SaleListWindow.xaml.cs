using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using System.Diagnostics;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using System.IO;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {

        private ISaleService _service = new SaleService();
        private IImageService _imageService = new ImageService();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        private IRFIDService _rfid = new RFIDService();

        public Sale SelectedSale { get; set; } = null;
        private MqttClientService _mqttClientService = new();
        private CustomerService customerService = new();
        private CameraService cameraService = new();
        private double? oldWeightValue = null;
        private DateTime? firstMessageTime = null;
        private string lastRFID = string.Empty;
        private string oldUrl1 = string.Empty;
        private string oldUrl2 = string.Empty;

        public Account CurrentAccount { get; set; } = null;
        public SaleListWindow()
        {
            InitializeComponent();
            
            LoadDataGrid();
            //try
            //{
            //    LoadAwait();
            //    _mqttClientService.MessageReceived += (s, data) =>
            //    {
            //        OnMqttMessageReceived(s, data);
            //    };
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

            //    BrokerWindow brokerWindow = new BrokerWindow();
            //    brokerWindow.ShowDialog();
            //    this.Close();
            //    return;
            //}
        }

        private async void LoadAwait()
        {
            await _mqttClientService.ConnectAsync();
            await _mqttClientService.SubscribeAsync("Can_ta");
            await _mqttClientService.SubscribeAsync("Can_tieu_ly");
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.CurrentAccount = CurrentAccount;
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {

            Sale selected = SaleDataGrid.SelectedItem as Sale;

            if (selected == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectSale, Constants.ErrorTitleSelectSale, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.SelectedSale = selected;
            saleManagementWindow.CurrentAccount = CurrentAccount;
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private async void LoadDataGrid()
        {
            var sales = await _service.GetAllSaleAsync(_currentPage, _pageSize);
            int totalSalesCount = await _service.GetTotalSalesCountAsync();
            _totalPages = (int)Math.Ceiling((double)totalSalesCount / _pageSize);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SaleDataGrid.ItemsSource = null;
                SaleDataGrid.Items.Clear();
                SaleDataGrid.ItemsSource = sales;
            });

            PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";

            // Disable/Enable pagination buttons
            PreviousPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }



        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadDataGrid();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadDataGrid();
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                AddSaleButton.Visibility = Visibility.Collapsed;
                EditSaleButton.Visibility = Visibility.Collapsed;
            }
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đang ở cửa sổ Quản lý Sale!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            Close();
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {

            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.ShowDialog();
            Close();
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            Close();
            roleListWindow.ShowDialog();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadDataGrid();
            }
            else
            {
                SaleDataGrid.ItemsSource = null;
                SaleDataGrid.Items.Clear();
                var sales = await _service.GetAllSaleAsync(1, 10);
                SaleDataGrid.ItemsSource = sales.Where(s => s.CustomerName.ToLower().Contains(searchTerm));
            }
        }

        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                CameraService cameraService = new();
                Camera newestCamera = await cameraService.GetNewestCameraAsync();

                if (newestCamera == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Không thể lấy thông tin từ Camera.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                if (data.StartsWith("Can_ta:"))
                {
                    string messageContent = data.Substring("Can_ta:".Length);
                    string filePathUrl = CaptureImageFromCamera(newestCamera, 1);
                    ProcessMqttMessage(messageContent, "RFID", "Weight");
                }
                else if (data.StartsWith("Can_tieu_ly:"))
                {
                    string messageContent = data.Substring("Can_tieu_ly:".Length);
                    string filePathUrl = CaptureImageFromCamera(newestCamera, 2);
                    ProcessMqttMessage(messageContent, "RFID", "Density");
                }
                else
                {
                    Debug.WriteLine("Unexpected message topic.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey)
        {
            try
            {
                // Split message by :
                string[] messages = messageContent.Split(':');

                Camera newestCamera = await cameraService.GetNewestCameraAsync();

                if (messages.Length == 2)
                {
                    string rfidValue = messages[0];
                    float currentValue = float.Parse(messages[1]);
                    Debug.Write(messageContent);
                    DateTime currentTime = DateTime.Now;
                    Sale sale = null;

                    if (firstKey == "RFID" && secondKey == "Weight")
                    {
                        sale = await _service.GetSaleByRFIDCodeWithoutDensity(rfidValue);
                        if (sale == null)
                        {
                            Customer customer = await customerService.GetCustomerByRFIDCodeAsync(rfidValue);
                            sale = new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidValue,
                                ProductWeight = currentValue,
                                LastEditedTime = currentTime,
                                ProductDensity = null,
                                Status = 1,
                                CustomerName = customer.CustomerName
                            };
                            Debug.Write("Sale current Weight" + sale.ProductWeight);
                            await _service.CreateSaleAsync(sale);
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
                            oldWeightValue = currentValue;
                            firstMessageTime = currentTime;
                            lastRFID = rfidValue;
                        }
                        else if (lastRFID == rfidValue && oldWeightValue.HasValue && firstMessageTime.HasValue)
                        {
                            sale = await _service.GetSaleByRFIDCodeWithoutDensity(rfidValue);
                            if (sale != null && !sale.ProductDensity.HasValue)
                            {
                                if (sale.ProductWeight.HasValue)
                                {
                                    currentValue += sale.ProductWeight.Value;
                                    sale.ProductWeight = currentValue;
                                    await _service.UpdateSaleAsync(sale);
                                }
                            }
                            String imagePath = CaptureImageFromCamera(newestCamera, 1);
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
                        }
                        oldWeightValue = currentValue;
                        firstMessageTime = currentTime;
                        lastRFID = rfidValue;
                    }
                    else if (firstKey == "RFID" && secondKey == "Density")
                    {
                        sale = await _service.GetSaleByRFIDCodeWithoutDensity(rfidValue);

                        if (sale != null && sale.ProductWeight.HasValue && !sale.ProductDensity.HasValue)
                        {
                            currentValue = float.Parse(messages[1]);
                            sale.ProductDensity = currentValue;
                            await _service.UpdateSaleAsync(sale);

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
                        }
                        else
                        {
                            throw new Exception("Sale with the specified RFID was not found.");
                        }
                    }

                    SaleDataGrid.Dispatcher.Invoke(() =>
                    {
                        LoadDataGrid();
                    });
                }
                else
                {
                    Debug.WriteLine("Định dạng tin nhắn không chính xác, không thể phân tích.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi xử lý nội dung tin nhắn: {ex.Message}");
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

        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaleDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch từ danh sách.", "Không có giao dịch được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Sale sale = SaleDataGrid.SelectedItem as Sale;
            IEnumerable<WPF_NhaMayCaoSu.Repository.Models.Image> images = await _imageService.Get2LatestImagesBySaleIdAsync(sale.SaleId);

            if (sale == null)
            {
                MessageBox.Show("Giao dịch được chọn không hợp lệ hoặc chưa được tải đúng cách.", "Giao dịch không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (images.Count() == 0)
            {
                MessageBox.Show("Giao dịch này không có hình ảnh nào được liên kết để hiển thị.", "Không tìm thấy hình ảnh", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ViewImagesWindow window = new(sale);
            window.ShowDialog();
        }

    }
}
