using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
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
        private readonly CameraService _cameraService = new CameraService();
        private readonly IBoardService _boardService = new BoardService();
        private readonly MqttClientService _mqttClientService = new MqttClientService();
        private readonly MqttServerService _mqttServerService = MqttServerService.Instance;

        // Pagination variables
        private int _currentPage = 1;
        private int _pageSize = 14;
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
            LoadDataGrid();

        }

        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        // Initializes the data grid and loads sales data
        private async void LoadDataGrid()
        {
            try
            {
                var allSales = await _saleService.GetAllSaleAsync();

                var sortedSales = allSales
                    .OrderByDescending(s => s.LastEditedTime)
                    .ThenByDescending(s => s.Status)
                    .ThenBy(s => s.CustomerName)
                    .ToList();

                int totalSalesCount = sortedSales.Count;
                _totalPages = (int)Math.Ceiling((double)totalSalesCount / _pageSize);

                var paginatedSales = sortedSales
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SaleDataGrid.ItemsSource = paginatedSales;
                    PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
                    PreviousPageButton.IsEnabled = _currentPage > 1;
                    NextPageButton.IsEnabled = _currentPage < _totalPages;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading sales data: {ex.Message}");
                Log.Error(ex, "Error loading sales data");
            }
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
        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, Camera newestCamera, short cameraIndex)
        {
            try
            {
                string[] messages = messageContent.Split('-');

                if (messages.Length != 4) return;

                string macAddress = messages[3];
                Board board = await _boardService.GetBoardByMacAddressAsync(macAddress);
                if (board == null)
                {
                    MessageBox.Show($"Dữ liệu được gửi từ board chưa được lưu trong cơ sở dữ liệu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string rfid = messages[0];
                float newValue = float.Parse(messages[1]); ;

                Sale sale = await _saleService.GetSaleByRFIDCodeWithoutDensity(rfid);

                if (sale == null)
                {
                    Customer customer = await _customerService.GetCustomerByRFIDCodeAsync(rfid);
                    RFID rfid_id = await _rfidService.GetRFIDByRFIDCodeAsync(rfid);
                    if (rfid_id.Status == 0)
                    {
                        MessageBox.Show("RFID đã hết hạn hoặc bị xóa, vui lòng sử dụng RFID khác", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (customer == null)
                    {
                        MessageBox.Show($"RFID {rfid} này chưa được tạo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        sale = await CreateNewSale(customer, rfid, newValue, secondKey, rfid_id);
                    }
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
                Log.Error(ex, "Error processing message");
            }
        }


        // Creates a new sale when an existing one is not found
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
            return sale;
        }

        // Captures an image from the camera and returns the file path
        private string CaptureImageFromCamera(Camera camera, int cameraIndex)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "CanCaoSu");
            Directory.CreateDirectory(folderPath);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string localFilePath = Path.Combine(folderPath, $"{Guid.NewGuid()}_Camera{cameraIndex}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (string.IsNullOrEmpty(cameraUrl)) throw new Exception($"URL của Camera {cameraIndex} không hợp lệ.");

                using (var capture = new VideoCapture(cameraUrl))
                {
                    if (!capture.IsOpened) throw new Exception($"Không thể mở Camera {cameraIndex}.");
                    using (var frame = new Mat())
                    {
                        capture.Read(frame);
                        if (frame.IsEmpty) throw new Exception($"Không thể chụp ảnh từ Camera {cameraIndex}.");

                        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
                        Bitmap bitmap = image.ToBitmap();
                        bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh từ Camera {cameraIndex}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, $"Lỗi khi chụp ảnh từ Camera {cameraIndex}");
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
            string searchTerm = SearchTextBox.Text.Trim().ToLower();
            SaleDataGrid.ItemsSource = null;

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadDataGrid();
            }
            else
            {
                var sales = await _saleService.GetAllSaleAsync(1, 10);
                SaleDataGrid.ItemsSource = sales.Where(s => s.CustomerName.ToLower().Contains(searchTerm));
            }
        }

        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedSale = SaleDataGrid.SelectedItem as Sale;
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

            var viewImagesWindow = new ViewImagesWindow(selectedSale);
            viewImagesWindow.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Sale selectedSale = SaleDataGrid.SelectedItem as Sale;

            if (selectedSale == null)
            {
                MessageBox.Show("Vui là chọn một Sale để xóa.", "Chọn Sale",.ServiceModel.OK, MessageBoxImage.Warning);
                return;
            }    
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa Sale này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                _saleService.DeleteSaleAsync(selectedSale.SaleId);
                MessageBox.Show("Đã xóa Sale thành công", "Thành công", MessageBoxButton.OK);
            }
        }
    }
}
