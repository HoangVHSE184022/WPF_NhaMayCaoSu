using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;


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
        private readonly IConfigService _cameraService = new ConfigService();
        private readonly IImageService _imageService = new ImageService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly ConfigService _localCameraService = new ConfigService();
        private readonly IBoardService _boardService = new BoardService();
        private readonly IRFIDService _rfidService = new RFIDService();
        private readonly IConfigService _configService = new ConfigService();

        private bool isExpanded = false;
        private Sale SaleDB { get; set; } = new();
        public List<Sale> _sessionSaleList { get; set; } = new();
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
            CheckBoardMode();
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
                Config newestCamera = await _cameraService.GetNewestCameraAsync();
                if (newestCamera == null)
                {
                    ShowError("Không thể lấy thông tin từ Config.");
                    return;
                }

                if (SharedTimerService.Instance.IsCountingDown)
                {
                    await _mqttClientService.PublishAsync(topic, payload);
                    MessageBox.Show("Đang trong thời gian không thể nhận tin nhắn từ MQTT. Vui lòng thử lại sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (data.Contains("Weight"))
                {
                    ProcessMqttMessage(data["info-".Length..], "RFID", "Weight", newestCamera, 1);
                    SharedTimerService.Instance.StartCountdown(newestCamera.Time);
                    await _mqttClientService.PublishAsync(topic, payload);
                }
                else if (data.Contains("Density"))
                {
                    ProcessMqttMessage(data["info-".Length..], "RFID", "Density", newestCamera, 2);
                    SharedTimerService.Instance.StartCountdown(newestCamera.Time);
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
        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, Config newestCamera, short cameraIndex)
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
                Config config = await _configService.GetNewestCameraAsync();
                float generalSalePrice = config.GeneralPrice;

                Board board = await _boardService.GetBoardByMacAddressAsync(macaddress);
                if (board == null)
                {
                    MessageBox.Show($"Board chứa MacAddress {macaddress} này chưa được tạo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                RFID rfidEntity = await _rfidService.GetRFIDByRFIDCodeAsync(rfid);
                if (rfidEntity == null)
                {
                    MessageBox.Show($"RFID {rfid} này không khả dụng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Sale sale = _sessionSaleList
                                .Where(s => s.RFIDCode == rfid)
                                .OrderByDescending(s => s.LastEditedTime)
                                .FirstOrDefault();

                if (sale == null)
                {
                    sale = await _saleService.GetSaleByRFIDCodeWithoutDensity(rfid);
                }

                DateTime currentTime = DateTime.Now;
                var latestSale = await _saleService.GetLatestSaleWithinTimeRangeAsync(currentTime.AddMinutes(-5), currentTime);
                bool otherRfidSaleExists = latestSale != null && !string.Equals(latestSale.RFIDCode, rfid, StringComparison.OrdinalIgnoreCase);
                bool isSaleCompleted = sale != null && sale.ProductWeight.HasValue && sale.ProductDensity != 0 && sale.TareWeight.HasValue;

                if (sale == null || isSaleCompleted || otherRfidSaleExists)
                {
                    Customer customer = await _customerService.GetCustomerByRFIDCodeAsync(rfid);

                    if (customer == null)
                    {
                        MessageBox.Show($"RFID {rfid} này chưa được tạo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        sale = await CreateNewSale(customer, rfid, newValue, secondKey, rfidEntity, generalSalePrice);
                    }
                }
                else
                {
                    // Update existing sale if found
                    if (secondKey == "Weight")
                    {
                        if (sale.ProductWeight.HasValue)
                        {
                            sale.LastEditedTime = DateTime.Now;

                            if ((currentTime - sale.LastEditedTime.Value).TotalMinutes <= 5)
                            {
                                sale.ProductWeight += newValue;
                            }
                            else
                            {
                                Customer customer = await _customerService.GetCustomerByRFIDCodeAsync(rfid);
                                sale = await CreateNewSale(customer, rfid, newValue, secondKey, sale.RFID, generalSalePrice);
                            }
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

                        if (sale.ProductWeight.HasValue &&
                       sale.TareWeight.HasValue &&
                       sale.ProductDensity.HasValue &&
                       sale.SalePrice.HasValue &&
                       sale.BonusPrice.HasValue)
                        {
                            if (!sale.TotalPrice.HasValue || sale.TotalPrice == 0)
                            {
                                CalculateTotalPrice(sale);

                                try
                                {
                                    await _saleService.UpdateSaleAsync(sale);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Failed to update sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Log.Error(ex, $"Error update sale: {messageContent}");
                                }
                            }
                        }
                    }

                    await _saleService.UpdateSaleAsync(sale);

                    var saleInSession = _sessionSaleList.FirstOrDefault(s => s.SaleId == sale.SaleId);
                    if (saleInSession != null)
                    {
                        _sessionSaleList.Remove(saleInSession);
                    }
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




        private async Task<Sale> CreateNewSale(Customer customer, string rfid, float value, string valueType, RFID rfid_id, float generalSalePrice)
        {
            Sale sale = new Sale
            {
                SaleId = Guid.NewGuid(),
                RFIDCode = rfid,
                RFID_Id = rfid_id.RFID_Id,
                CustomerName = customer.CustomerName,
                BonusPrice = customer.bonusPrice,
                SalePrice = generalSalePrice,
                CreatedDate = DateTime.Now,
                LastEditedTime = DateTime.Now,
                Status = 1,
                TotalPrice = 0
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
        private string CaptureImageFromCamera(Config camera, int cameraIndex)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CaoSuPictures");
            Directory.CreateDirectory(folderPath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string localFilePath = Path.Combine(folderPath, $"{Guid.NewGuid()}_Camera{cameraIndex}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (string.IsNullOrEmpty(cameraUrl)) throw new Exception($"URL của Config {cameraIndex} không hợp lệ.");

                using (var capture = new VideoCapture(cameraUrl))
                {
                    if (!capture.IsOpened) throw new Exception($"Không thể mở Config {cameraIndex}.");
                    using (var frame = new Mat())
                    {
                        capture.Read(frame);
                        if (frame.IsEmpty) throw new Exception($"Không thể chụp ảnh từ Config {cameraIndex}.");

                        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
                        Bitmap bitmap = image.ToBitmap();
                        bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh từ Config {cameraIndex}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, $"Lỗi khi chụp ảnh từ Config {cameraIndex}");
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

        public void LoadDataGrid()
        {
            SalesDataGrid.Dispatcher.Invoke(() =>
            {
                SalesDataGrid.ItemsSource = null;
                SalesDataGrid.ItemsSource = _sessionSaleList;
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
            SharedTimerService.Instance.TimerTicked += OnTimerTicked;
            SharedTimerService.Instance.TimerEnded += OnTimerEnded;
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
        private void OnTimerTicked(object sender, int remainingSeconds)
        {
            Dispatcher.Invoke(() =>
            {
                if (remainingSeconds > 0)
                {
                    AbleScanLabel.Content = $"Hiện tại không thể quét. Thử lại sau {remainingSeconds} giây.";
                    AbleScanLabel.Foreground = new SolidColorBrush(Colors.Red);
                }
            });
        }

        private void OnTimerEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                AbleScanLabel.Content = "Có thể quét lại.";
                AbleScanLabel.Foreground = new SolidColorBrush(Colors.Green);
            });
        }

        private void OpenBrokerWindow()
        {
            var brokerWindow = new BrokerWindow();
            brokerWindow.ShowDialog();
            Close();
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
                _sessionSaleList = _sessionSaleList,
                CurrentAccount = CurrentAccount,
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
            Close();
        }

        private async void CheckBoardMode()
        {
            Board boardTa = await _boardService.GetBoardByNameAsync("Cân Tạ");

            Board boardTieuLy = await _boardService.GetBoardByNameAsync("Cân Tiểu Ly");

            if (boardTa != null)
            {
                if (boardTa.BoardMode == 2)
                {
                    MessageBox.Show("Board Cân tạ đang ở Mode 2 sẽ được chuyển sang Mode 1", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    boardTa.BoardMode = 1;
                    await _boardService.UpdateBoardAsync(boardTa);

                    string topic = $"{boardTa.BoardMacAddress}/mode";


                    var payloadObject = new { Mode = boardTa.BoardMode };
                    string payload = JsonConvert.SerializeObject(payloadObject);

                    if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                    {
                        await _mqttClientService.PublishAsync(topic, payload);
                        await _boardService.UpdateBoardAsync(boardTa);
                    }
                }
            }
            if (boardTieuLy != null)
            {
                if (boardTieuLy.BoardMode == 2)
                {
                    MessageBox.Show("Board Cân tiểu ly đang ở Mode 2 sẽ được chuyển sang Mode 1", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    boardTieuLy.BoardMode = 1;
                    await _boardService.UpdateBoardAsync(boardTieuLy);

                    string topic = $"{boardTieuLy.BoardMacAddress}/mode";


                    var payloadObject = new { Mode = boardTieuLy.BoardMode };
                    string payload = JsonConvert.SerializeObject(payloadObject);

                    if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                    {
                        await _mqttClientService.PublishAsync(topic, payload);
                        await _boardService.UpdateBoardAsync(boardTieuLy);
                    }
                }
            }

        }

        private async void SalesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedSale = e.Row.Item as Sale;
            if (editedSale == null) return;

            var editedElement = e.EditingElement as TextBox;
            string editedValue = editedElement?.Text;

            var editedColumn = e.Column.Header.ToString();

            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn thay đổi {editedColumn} thành {editedValue}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    switch (editedColumn)
                    {
                        case "Số kí":
                            if (string.IsNullOrWhiteSpace(editedValue) || !float.TryParse(editedValue, out float productWeight))
                            {
                                MessageBox.Show("Vui lòng nhập một số hợp lệ cho Số ký.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }
                            editedSale.ProductWeight = productWeight;
                            CalculateTotalPrice(editedSale);
                            break;

                        case "Tỉ Trọng":
                            if (string.IsNullOrWhiteSpace(editedValue) || !float.TryParse(editedValue, out float productDensity))
                            {
                                MessageBox.Show("Vui lòng nhập một số hợp lệ cho Tỉ trọng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }
                            if (productDensity > 1)
                            {
                                MessageBox.Show("Tỉ trọng không thể vượt quá 100%", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }
                            editedSale.ProductDensity = productDensity;
                            CalculateTotalPrice(editedSale);
                            break;

                        case "Số bì":
                            if (string.IsNullOrWhiteSpace(editedValue) || !float.TryParse(editedValue, out float tareWeight))
                            {
                                MessageBox.Show("Vui lòng nhập một số hợp lệ cho Số bì.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                break;
                            }
                            editedSale.TareWeight = tareWeight;
                            CalculateTotalPrice(editedSale);
                            break;

                        default:
                            return;
                    }

                    editedSale.LastEditedTime = DateTime.Now;

                    await _saleService.UpdateSaleAsync(editedSale);
                    LoadDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void CalculateTotalPrice(Sale sale)
        {
            if (sale != null)
            {
                float productWeight = sale.ProductWeight ?? 0;
                float tareWeight = sale.TareWeight ?? 0;
                float productDensity = sale.ProductDensity ?? 0;
                float salePrice = sale.SalePrice ?? 0;
                float bonusPrice = sale.BonusPrice ?? 0;

                sale.TotalPrice = (productWeight - tareWeight) * productDensity * (salePrice + bonusPrice);
            }
        }

    }
}

