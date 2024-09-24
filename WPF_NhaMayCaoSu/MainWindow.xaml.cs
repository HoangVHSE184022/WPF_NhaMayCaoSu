using Emgu.CV;
using Emgu.CV.Structure;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using Serilog;
using Azure.Messaging;
using Newtonsoft.Json;


namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Services
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private readonly ISaleService _saleService = new SaleService();
        private readonly ICameraService _cameraService = new CameraService();
        private readonly IImageService _imageService = new ImageService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly CameraService _localCameraService = new CameraService();
        private readonly IBoardService _boardService = new BoardService();
        private readonly IRFIDService _rfidService = new RFIDService();

        private bool isExpanded = false;
        private Sale SaleDB { get; set; } = new();
        private List<Sale> _sessionSaleList { get; set; } = new();
        public Account CurrentAccount { get; set; } = null;
        private readonly BrokerWindow broker;
        private bool isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
            broker = new BrokerWindow();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            LoadDataGrid();

        }

        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        // Handle incoming sales data and update the UI
        private async void OnMqttMessageReceived(object sender, string data)
        {
            var value = new { Save = 1 };
            var payloadObject = value;
            string[] messages = data["info-".Length..].Split('-');
            string macAddress = messages[3];
            string topic = $"{macAddress}/Save";
            string payload = JsonConvert.SerializeObject(payloadObject);
            try
            {
                Camera newestCamera = await _cameraService.GetNewestCameraAsync();
                if (newestCamera == null)
                {
                    ShowError("Không thể lấy thông tin từ Camera.");
                    return;
                }

                if (data.Contains("Weight"))
                {
                    ProcessMqttMessage(data["info-".Length..], "RFID", "Weight", newestCamera, 1);
                    await _mqttClientService.PublishAsync(topic, payload);
                }
                else if (data.Contains("Density"))
                {
                    ProcessMqttMessage(data["info-".Length..], "RFID", "Density", newestCamera, 2);
                    await _mqttClientService.PublishAsync(topic, payload);
                }
                else
                {
                    Debug.WriteLine("Unexpected message topic.");
                    await _mqttClientService.PublishAsync(topic, payload);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing MQTT message: {ex.Message}");
                Log.Error(ex, $"Error processing MQTT message: {data}");
                if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                {
                    try
                    {
                        await _mqttClientService.PublishAsync(topic, payload);
                    }
                    catch (Exception publishEx)
                    {
                        Debug.WriteLine($"Error publishing message in catch block: {publishEx.Message}");
                        Log.Error(publishEx, "Error publishing message in catch block");
                    }
                }
            }
        }

        // Processes the MQTT message and updates the sale
        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, Camera newestCamera, short cameraIndex)
        {
            try
            {
                string[] messages = messageContent.Split('-');

                if (messages.Length != 4)
                {
                    return;
                }
                string rfid = messages[0];
                float newValue = float.Parse(messages[1]);
                string macaddress = messages[3];

                Sale sale = await _saleService.GetSaleByRFIDCodeWithoutDensity(rfid);
                Board board = await _boardService.GetBoardByMacAddressAsync(macaddress);

                if (board == null)
                {
                    MessageBox.Show($"Board chứa MacAddress {macaddress} này chưa được tạo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                if (sale == null)
                {
                    Customer customer = await _customerService.GetCustomerByRFIDCodeAsync(rfid);
                    RFID rfid_id = await _rfidService.GetRFIDByRFIDCodeAsync(rfid);

                    if (customer == null)
                    {
                        MessageBox.Show($"RFID {rfid} này chưa được tạo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    sale = await CreateNewSale(customer, rfid, newValue, secondKey, rfid_id);

                }
                else
                {

                    if (secondKey == "Weight")
                    {
                        if (sale.ProductWeight.HasValue)
                        {
                            sale.LastEditedTime = DateTime.Now;
                            sale.ProductWeight += newValue;
                        }
                        else
                        {
                            sale.LastEditedTime = DateTime.Now;
                            sale.ProductWeight = newValue;
                        }
                    }
                    else if (secondKey == "Density")
                    {
                        if (newValue > 100)
                        {
                            MessageBox.Show("Tỉ trọng không thể vượt quá 100 %", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        if (sale.ProductDensity == 0)
                        {
                            sale.LastEditedTime = DateTime.Now;
                            sale.ProductDensity = newValue;
                        }
                    }

                    await _saleService.UpdateSaleAsync(sale);
                    _sessionSaleList.Add(sale);
                }

                string imagePath = CaptureImageFromCamera(newestCamera, cameraIndex);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    await SaveImageToDb(imagePath, sale.SaleId, cameraIndex);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadDataGrid();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
                Log.Error(ex, $"Error processing message: {messageContent}");
            }
        }

        private async Task<Sale> CreateNewSale(Customer customer, string rfid, float value, string valueType, RFID rfid_id)
        {
            var sale = new Sale
            {
                SaleId = Guid.NewGuid(),
                RFIDCode = rfid,
                RFID_Id = rfid_id.RFID_Id,
                CustomerName = customer.CustomerName,
                LastEditedTime = DateTime.Now,
                Status = 1
            };

            if (valueType == "Weight")
            {
                sale.ProductWeight = value;
                sale.ProductDensity = 0;
            }
            else
                sale.ProductDensity = value;

            await _saleService.CreateSaleAsync(sale);
            _sessionSaleList.Add(sale);
            return sale;
        }


        // Capture image from the camera and return the file path
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

                        // Convert frame to image and save
                        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
                        Bitmap bitmap = image.ToBitmap();
                        bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh từ Camera {cameraIndex}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, $"Error taking picture from Camera: {cameraIndex}");
                return string.Empty;
            }

            return localFilePath;
        }

        // Save captured image to the database
        private async Task SaveImageToDb(string imagePath, Guid saleId, short imageType)
        {
            if (!string.IsNullOrEmpty(imagePath))
            {
                var image = new Repository.Models.Image
                {
                    ImageId = Guid.NewGuid(),
                    ImageType = imageType,
                    ImagePath = imagePath,
                    CreatedDate = DateTime.Now,
                    SaleId = saleId
                };
                await _imageService.AddImageAsync(image);
            }
        }

        // Update the SalesDataGrid with the latest session sales
        private void LoadDataGrid()
        {
            SalesDataGrid.Dispatcher.Invoke(() =>
            {
                SalesDataGrid.ItemsSource = null;
                SalesDataGrid.ItemsSource = _sessionSaleList;
                Debug.WriteLine(_sessionSaleList.Count);
            });
        }

        // Quit the application
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Hành động này sẽ đóng ứng dụng, bạn chắc chứ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }
        }

        // Window loaded event to handle the connection to the MQTT broker
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isExpanded == true)
            {
                ExpandButton.Visibility = Visibility.Hidden;
                CloseButton.Visibility = Visibility.Visible;
            }

            try
            {
                if (!_mqttClientService.IsConnected)
                {
                    await _mqttClientService.ConnectAsync();
                    await _mqttClientService.SubscribeAsync("+/info");
                }

                if (!isLoaded)
                {
                    _mqttClientService.MessageReceived += OnMqttMessageReceived;
                    isLoaded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, $"Không thể kết nối đến máy chủ MQTT");
                OpenBrokerWindow();
            }

            LoadDataGrid();
        }

        private void OpenBrokerWindow()
        {
            var brokerWindow = new BrokerWindow();
            brokerWindow.ShowDialog();
            this.Close();
        }

        // Event handlers for buttons
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


        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow viewWindow = new MainWindow
            {
                _sessionSaleList = this._sessionSaleList,
                CurrentAccount = this.CurrentAccount,
                WindowState = WindowState.Maximized,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                isExpanded = true
            };

            viewWindow.Show();
        }
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
