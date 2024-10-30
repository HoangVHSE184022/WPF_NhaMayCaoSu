using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {
        // Services
        private readonly ISaleService _saleService = new SaleService();
        private readonly IImageService _imageService = new ImageService();
        private readonly IRFIDService _rfidService = new RFIDService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly ConfigService _cameraService = new ConfigService();
        private readonly IBoardService _boardService = new BoardService();
        private readonly MqttClientService _mqttClientService = new MqttClientService();
        private readonly MqttServerService _mqttServerService = MqttServerService.Instance;
        private readonly IConfigService _configService = new ConfigService();

        // Pagination variables
        private int _currentPage = 1;
        private int _pageSize = 18;
        private int _totalPages;

        // State variables
        private double? _oldWeightValue = null;
        private DateTime? _firstMessageTime = null;
        private string _lastRFID = string.Empty;
        private bool isLoaded = false;
        private MainWindow _mainWindow;

        // Current Account
        public Account CurrentAccount { get; set; } = null;

        public SaleListWindow()
        {
            InitializeComponent();
            LoadDataGrid();
            LoggingHelper.ConfigureLogger();
        }

        public SaleListWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadDataGrid();
            LoggingHelper.ConfigureLogger();
            _mainWindow = mainWindow;
        }
        // Initializes and subscribes to the necessary MQTT topics
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SharedTimerService.Instance.TimerTicked += OnTimerTicked;
                SharedTimerService.Instance.TimerEnded += OnTimerEnded;
                
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
                ShowError("Không thể kết nối đến máy chủ MQTT. Bạn sẽ được chuyển về màn hình quản lý Broker.");
                Log.Error(ex, "Không thể kết nối đến máy chủ MQTT");
                OpenBrokerWindow();
            }
            FromDatePicker.SelectedDate = DateTime.Now.AddDays(-21);
            ToDatePicker.SelectedDate = DateTime.Now;
            CheckBoardMode();
            LoadDataGrid();

        }

        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private async void LoadDataGrid()
        {
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            string? customer = SearchTextBox.Text.Trim().ToLower();
            try
            {
                IEnumerable<Sale> allSales = await _saleService.GetAllSaleAsync(_currentPage, _pageSize);
                IEnumerable<Sale> allSalesCount = await _saleService.GetAllSaleAsync();

                List<Sale> salesList = allSales.ToList();
                if (fromDate != null && toDate != null)
                {
                    salesList = salesList.Where(s => s.CreatedDate >= fromDate && s.CreatedDate <= toDate).ToList();
                }
                if (customer != null)
                {
                    salesList = salesList.Where(s => s.CustomerName.ToLower().Contains(customer)).ToList();
                }

                int totalSalesCount = allSalesCount.Count();
                Debug.WriteLine($"Count {totalSalesCount}");
                _totalPages = (int)Math.Ceiling((double)totalSalesCount / _pageSize);
                Debug.WriteLine($"TotalPages {_totalPages}");
                SaleDataGrid.ItemsSource = salesList;
                PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
                PreviousPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sales data: {ex.Message}");
                Log.Error(ex, "Error loading sales data");
            }
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

        // Handles receiving an MQTT message and processes it
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
                Log.Error(ex, "Error processing MQTT message");
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

                Sale sale = await _saleService.GetSaleByRFIDCodeWithoutDensity(rfid);
                DateTime currentTime = DateTime.Now;
                RFID rfidEntity = await _rfidService.GetRFIDByRFIDCodeAsync(rfid);
                if (rfidEntity == null)
                {
                    MessageBox.Show($"RFID {rfid} này không khả dụng", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                Sale latestSale = await _saleService.GetLatestSaleWithinTimeRangeAsync(currentTime.AddMinutes(-5), currentTime);
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
                    if (secondKey == "Weight")
                    {
                        // Update the weight if sale already has a weight value
                        if (sale.ProductWeight.HasValue)
                        {
                            sale.LastEditedTime = DateTime.Now;

                            // If the last update is within 5 minutes, update weight
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
                        // Update the density only if it's within valid range and hasn't been set before
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
                    _mainWindow._sessionSaleList.Add(sale);
                    _mainWindow.LoadDataGrid();
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



        // Creates a new sale when an existing one is not found
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
            return sale;
        }

        // Captures an image from the camera and returns the file path
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

        // Saves the image to the database
        private async Task SaveImageToDb(string imagePath, Guid saleId, short imageType)
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

        // Helper method to show error message
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Button click handlers
        private void AddSaleButton_Click(object sender, RoutedEventArgs e) => OpenSaleManagementWindow();
        private void EditSaleButton_Click(object sender, RoutedEventArgs e) => OpenEditSaleWindow();
        private void QuitButton_Click(object sender, RoutedEventArgs e) => Close();

        private void OpenSaleManagementWindow()
        {
            var saleManagementWindow = new SaleManagementWindow(_mqttClientService, _mainWindow)
            {
                CurrentAccount = CurrentAccount
            };
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void OpenEditSaleWindow()
        {
            Sale selectedSale = SaleDataGrid.SelectedItem as Sale;
            if (selectedSale == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectSale, Constants.ErrorTitleSelectSale, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            var saleManagementWindow = new SaleManagementWindow(_mqttClientService, _mainWindow)
            {
                SelectedSale = selectedSale,
                CurrentAccount = CurrentAccount
            };
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void OpenBrokerWindow()
        {
            var brokerWindow = new BrokerWindow
            {
                CurrentAccount = CurrentAccount
            };
            brokerWindow.ShowDialog();
            Close();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            
        }

        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            Sale selectedSale = SaleDataGrid.SelectedItem as Sale;
            if (selectedSale == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch từ danh sách.", "Không có giao dịch được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var images = await _imageService.Get2LatestImagesBySaleIdAsync(selectedSale.SaleId);
            if (!images.Any())
            {
                MessageBox.Show("Giao dịch này không có hình ảnh nào được liên kết để hiển thị.", "Không tìm thấy hình ảnh", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ViewImagesWindow viewImagesWindow = new ViewImagesWindow(selectedSale);
            viewImagesWindow.ShowDialog();
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Sale selectedSale = SaleDataGrid.SelectedItem as Sale;
            if (selectedSale == null)
            {
                MessageBox.Show("Vui là chọn một Sale để xóa.", "Chọn Sale", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa Sale này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                await _saleService.DeleteSaleAsync(selectedSale.SaleId);
                LoadDataGrid();
                _mainWindow._sessionSaleList.Remove(selectedSale);
                _mainWindow.LoadDataGrid();
                MessageBox.Show("Đã xóa Sale thành công", "Thành công", MessageBoxButton.OK);
            }
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

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sales = await _saleService.GetAllSaleAsync();
                var filteredSales = sales.ToList();

                if (filteredSales.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sales Data");

                    var header = new List<string> { "Số thứ tự", "SaleId", "Tên khách hàng", "Tỉ trọng", "Cân nặng", "Thời gian chỉnh sửa cuối", "Mã RFID", "Hình ảnh (Tỉ trọng)", "Hình ảnh (Cân nặng)" };

                    for (int i = 0; i < header.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = header[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i + 1].AutoFitColumns();
                    }

                    for (int i = 0; i < filteredSales.Count; i++)
                    {
                        var sale = filteredSales[i];

                        var images = await _imageService.GetImagesBySaleIdAsync(sale.SaleId);
                        string densityImageUrl = images.FirstOrDefault(img => img.ImageType == 2)?.ImagePath ?? "N/A";
                        string weightImageUrl = images.FirstOrDefault(img => img.ImageType == 1)?.ImagePath ?? "N/A";

                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = sale.SaleId;
                        worksheet.Cells[i + 2, 3].Value = sale.CustomerName;
                        worksheet.Cells[i + 2, 4].Value = sale.ProductDensity;
                        worksheet.Cells[i + 2, 5].Value = sale.ProductWeight;
                        worksheet.Cells[i + 2, 6].Value = sale.LastEditedTime.HasValue ? sale.LastEditedTime.Value.ToString("g") : "N/A";
                        worksheet.Cells[i + 2, 7].Value = sale.RFIDCode;
                        worksheet.Cells[i + 2, 8].Value = densityImageUrl;
                        worksheet.Cells[i + 2, 9].Value = weightImageUrl;
                    }

                    for (int col = 1; col <= 9; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CaoSuData");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, $"SalesData_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    File.WriteAllBytes(filePath, package.GetAsByteArray());

                    MessageBox.Show($"Xuất file Excel thành công tại: {filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình xuất file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Error exporting Sales to Excel");
            }
        }

        private async void SaleDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Get the edited sale
            var editedSale = e.Row.Item as Sale;
            if (editedSale == null) return;

            // Get the edited element and value
            var editedElement = e.EditingElement as TextBox;
            string editedValue = editedElement?.Text;

            // Get the edited column header
            var editedColumn = e.Column.Header.ToString();

            // Show confirmation dialog
            MessageBoxResult result = MessageBox.Show($"Bạn có chắc muốn thay đổi {editedColumn} thành {editedValue}?",
                                                      "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    switch (editedColumn)
                    {
                        case "Số ký":
                            editedSale.ProductWeight = float.Parse(editedValue);
                            break;
                        case "Tỉ trọng":
                            editedSale.ProductDensity = float.Parse(editedValue);
                            if (editedSale.ProductDensity > 100)
                            {
                                MessageBox.Show("Tỉ trọng không thể vượt quá 100%", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            break;
                        case "Số bì":
                            editedSale.TareWeight = float.Parse(editedValue);
                            CalculateTotalPrice(editedSale);
                            break;
                        default:
                            return;
                    }

                    // Update the LastEditedTime to now
                    editedSale.LastEditedTime = DateTime.Now;

                    // Update the sale in the database
                    await _saleService.UpdateSaleAsync(editedSale);

                    var saleInSession = _mainWindow._sessionSaleList.FirstOrDefault(s => s.SaleId == editedSale.SaleId);
                    if (saleInSession != null)
                    {
                        saleInSession.ProductWeight = editedSale.ProductWeight;
                        saleInSession.ProductDensity = editedSale.ProductDensity;
                        saleInSession.TareWeight = editedSale.TareWeight;
                        saleInSession.LastEditedTime = editedSale.LastEditedTime;
                        _mainWindow.LoadDataGrid();
                    }

                    LoadDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private async void CalculateTotalPrice_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Sale> saleList = await _saleService.GetSalesWithoutTotalPriceAsync();
            foreach (var sale in saleList)
            {
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
                        }
                    }
                }
            }

            LoadDataGrid();
            _mainWindow.LoadDataGrid();
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

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataGrid();
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataGrid();
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                LoadDataGrid();
            }
        }
    }
}
