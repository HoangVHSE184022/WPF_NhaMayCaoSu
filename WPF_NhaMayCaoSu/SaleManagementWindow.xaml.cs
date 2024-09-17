using System;
using System.Windows;
using System.Diagnostics;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    public partial class SaleManagementWindow : Window
    {
        private readonly ISaleService _service = new SaleService();
        private readonly IRFIDService _rfidService = new RFIDService();
        private readonly CameraService _cameraService = new CameraService();
        private readonly MqttClientService _mqttClientService = new MqttClientService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly SaleService _saleService = new SaleService();
        private readonly ImageService _imageService = new ImageService();

        // State variables
        private float? _oldWeightValue = null;
        private float? _oldDensityValue = null;
        private DateTime? _firstMessageTime = null;
        private string _lastRFID = string.Empty;
        private string _oldUrlWeight = string.Empty;
        private string _oldUrlDensity = string.Empty;

        public Sale SelectedSale { get; set; } = null;
        public Account CurrentAccount { get; set; } = null;

        public SaleManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e) => Close();

        // Save Button Click Handler
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateFormInput())
            {
                var sale = new Sale
                {
                    CustomerName = CustomerNameTextBox.Text,
                    ProductWeight = float.Parse(WeightTextBox.Text),
                    ProductDensity = string.IsNullOrWhiteSpace(DensityTextBox.Text) ? 0 : float.Parse(DensityTextBox.Text),
                    Status = short.Parse(StatusTextBox.Text),
                    RFIDCode = RFIDCodeTextBox.Text
                };

                if (SelectedSale == null)
                {
                    await _service.CreateSaleAsync(sale);
                    MessageBox.Show(Constants.SuccessMessageSaleCreated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    sale.SaleId = SelectedSale.SaleId;
                    sale.LastEditedTime = DateTime.UtcNow;
                    await _service.UpdateSaleAsync(sale);
                    MessageBox.Show(Constants.SuccessMessageSaleUpdated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
        }

        // Validate form inputs
        private bool ValidateFormInput()
        {
            if (string.IsNullOrWhiteSpace(RFIDCodeTextBox.Text))
            {
                MessageBox.Show("RFID không hợp lệ hoặc chưa được nhập. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(WeightTextBox.Text))
            {
                MessageBox.Show("Dữ liệu cân tạ không hợp lệ hoặc chưa được nhập. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (float.TryParse(DensityTextBox.Text, out float densityValue) && densityValue > 100)
            {
                MessageBox.Show("Tỉ trọng không thể vượt quá 100 %", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Window Loaded Event
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = SelectedSale == null ? Constants.ModeLabelAddSale : Constants.ModeLabelEditSale;

            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("+/info");
                _mqttClientService.MessageReceived += OnMqttMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            if (SelectedSale != null)
            {
                PopulateSaleDetails(SelectedSale);
                await LoadImagePaths(SelectedSale.SaleId);
            }
        }

        // Populate Sale details if editing an existing sale
        private void PopulateSaleDetails(Sale sale)
        {
            CustomerNameTextBox.Text = sale.CustomerName;
            RFIDCodeTextBox.Text = sale.RFIDCode;
            WeightTextBox.Text = sale.ProductWeight?.ToString();
            DensityTextBox.Text = sale.ProductDensity?.ToString();
            StatusTextBox.Text = sale.Status.ToString();
        }

        // Load Image Paths for the Sale
        private async Task LoadImagePaths(Guid saleId)
        {
            var images = await _imageService.GetImagesBySaleIdAsync(saleId);
            foreach (var image in images)
            {
                if (image.ImageType == 1)
                    URLWeightTextBox.Text = image.ImagePath;
                else if (image.ImageType == 2)
                    URLDensityTextBox.Text = image.ImagePath;
            }
        }

        // Handles incoming MQTT messages
        private async void OnMqttMessageReceived(object sender, string data)
        {
            Debug.WriteLine($"MQTT Message received: {data}");
            try
            {
                Camera newestCamera = await _cameraService.GetNewestCameraAsync();

                if (newestCamera == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Không thể lấy thông tin từ camera.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                if (data.Contains("Weight"))
                {
                    string messageContent = data["info-".Length..];
                    string filePathUrl = CaptureImageFromCamera(newestCamera, 1);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessMqttMessage(messageContent, "RFID", "Weight", RFIDCodeTextBox, WeightTextBox);
                        ProcessCameraUrlMessage(filePathUrl, RFIDCodeTextBox.Text, URLWeightTextBox);
                    });
                }
                else if (data.Contains("Density"))
                {
                    string messageContent = data["info-".Length..];
                    string filePathUrl = CaptureImageFromCamera(newestCamera, 2);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessMqttMessage(messageContent, "RFID", "Density", RFIDCodeTextBox, DensityTextBox);
                        ProcessCameraUrlMessage(filePathUrl, RFIDCodeTextBox.Text, URLDensityTextBox);
                    });
                }
                else
                {
                    Debug.WriteLine("Unknown MQTT topic.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        // Process the received MQTT message (weight or density)
        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, System.Windows.Controls.TextBox firstTextBox, System.Windows.Controls.TextBox secondTextBox)
        {
            try
            {
                string[] messages = messageContent.Split('-');
                if (messages.Length == 3)
                {
                    string rfidValue = messages[0];
                    string currentValueString = messages[1];
                    Customer customer = await _customerService.GetCustomerByRFIDCodeAsync(rfidValue);
                    string customerName = customer.CustomerName;

                    // Validate if the value can be parsed
                    if (!float.TryParse(currentValueString, out float currentValue))
                    {
                        throw new FormatException("Giá trị không hợp lệ. Không thể phân tích giá trị số từ dữ liệu nhận được.");
                    }

                    Debug.WriteLine($"Origin mes {messageContent}");
                    Debug.WriteLine($"1 mes {firstTextBox}");
                    Debug.WriteLine($"2 mes {rfidValue}");
                    Debug.WriteLine($"3 mes {currentValue}");

                    float existingValue = 0;

                    if (!string.IsNullOrEmpty(WeightTextBox.Text) && !float.TryParse(WeightTextBox.Text, out existingValue))
                    {
                        MessageBox.Show("Dữ liệu cân tạ không hợp lệ. Vui lòng kiểm tra lại.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    Debug.WriteLine($"Existing value: {existingValue}");

                    if (!string.IsNullOrEmpty(firstTextBox.Text))
                    {
                        if (secondKey == "Weight")
                        {
                            existingValue += currentValue; // Accumulate the weight
                        }
                        else if (secondKey == "Density")
                        {
                            existingValue = currentValue; // Set the density
                        }
                    }
                    else
                    {
                        firstTextBox.Text = rfidValue;

                        if (secondKey == "Weight")
                        {
                            existingValue = currentValue; // Set the weight
                        }
                    }

                    Debug.WriteLine($"Final value: {existingValue}");

                    // Update UI fields safely using Dispatcher
                    firstTextBox.Dispatcher.Invoke(() => firstTextBox.Text = rfidValue);
                    secondTextBox.Dispatcher.Invoke(() => secondTextBox.Text = existingValue.ToString());
                    CustomerNameTextBox.Dispatcher.Invoke(() => CustomerNameTextBox.Text=customerName);
                }
                else
                {
                    Debug.WriteLine("Invalid message format.");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Dữ liệu không hợp lệ: {ex.Message}", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                Debug.WriteLine($"FormatException: {ex.Message}");
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Không thể xử lý dữ liệu do trường không tồn tại hoặc rỗng.", "Lỗi tham chiếu null", MessageBoxButton.OK, MessageBoxImage.Warning);
                Debug.WriteLine($"NullReferenceException: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi trong quá trình xử lý tin nhắn MQTT: {ex.Message}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Error processing MQTT message: {ex.Message}");
            }
        }



        // Processes the image URL updates for the camera
        private void ProcessCameraUrlMessage(string cameraUrl, string rfidValue, System.Windows.Controls.TextBox urlTextBox)
        {
            try
            {
                DateTime currentTime = DateTime.UtcNow;
                urlTextBox.Dispatcher.Invoke(() =>
                {
                    if (_lastRFID == rfidValue && _firstMessageTime.HasValue)
                    {
                        // Append new URL to the existing URL
                        if (urlTextBox == URLWeightTextBox)
                        {
                            _oldUrlWeight = string.IsNullOrEmpty(_oldUrlWeight) ? cameraUrl : $"{_oldUrlWeight}\n{cameraUrl}";
                            urlTextBox.Text = _oldUrlWeight;
                        }
                        else if (urlTextBox == URLDensityTextBox)
                        {
                            _oldUrlDensity = string.IsNullOrEmpty(_oldUrlDensity) ? cameraUrl : $"{_oldUrlDensity}\n{cameraUrl}";
                            urlTextBox.Text = _oldUrlDensity;
                        }
                    }
                    else
                    {
                        // If it's a new RFID or more than 5 minutes have passed, reset the URLs
                        if (urlTextBox == URLWeightTextBox)
                        {
                            _oldUrlWeight = cameraUrl;
                            urlTextBox.Text = _oldUrlWeight;
                        }
                        else if (urlTextBox == URLDensityTextBox)
                        {
                            _oldUrlDensity = cameraUrl;
                            urlTextBox.Text = _oldUrlDensity;
                        }

                        // Update the last RFID and timestamp
                        _lastRFID = rfidValue;
                        _firstMessageTime = currentTime;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing camera URL message: {ex.Message}");
            }
        }

        // Capture image from the camera
        private string CaptureImageFromCamera(Camera camera, int cameraIndex)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hình ảnh cân cao su");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string localFilePath = Path.Combine(folderPath, $"{Guid.NewGuid()}_Camera{cameraIndex}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (string.IsNullOrEmpty(cameraUrl)) throw new Exception($"URL của Camera {cameraIndex} không hợp lệ.");

                using var capture = new VideoCapture(cameraUrl);
                if (!capture.IsOpened) throw new Exception($"Không thể mở Camera {cameraIndex}.");

                using var frame = new Mat();
                capture.Read(frame);
                if (frame.IsEmpty) throw new Exception($"Không thể chụp ảnh từ Camera {cameraIndex}.");

                var image = frame.ToImage<Bgr, byte>();
                Bitmap bitmap = image.ToBitmap();
                bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chụp ảnh từ Camera {cameraIndex}: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return localFilePath;
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
    }
}
