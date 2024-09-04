using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using System.Diagnostics;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleManagementWindow.xaml
    /// </summary>
    public partial class SaleManagementWindow : Window
    {

        private SaleService _service = new();

        public Account CurrentAccount { get; set; } = null;

        public Sale SelectedSale { get; set; } = null;
        private MqttClientService _mqttClientService = new MqttClientService();
        private CustomerService customerService = new CustomerService();
        private SaleService _saleService = new SaleService();
        private double? oldWeightValue = null;
        private DateTimeOffset? firstMessageTime = null;
        private string lastRFID = null;
        private string oldUrl1 = null;
        private string oldUrl2 = null;

        public SaleManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Sale x = new();

            x.CustomerName = CustomerNameTextBox.Text;
            x.ProductWeight = int.Parse(WeightTextBox.Text);
            x.ProductDensity = int.Parse(DensityTextBox.Text);
            x.Status = short.Parse(StatusTextBox.Text);
            x.RFIDCode = RFIDCodeTextBox.Text;


            CameraService cameraService = new CameraService();
            Camera newestCamera = await cameraService.GetNewestCameraAsync();
            string imageFilePath = string.Empty;


            if (SelectedSale == null)
            {
                MessageBox.Show(Constants.SuccessMessageSaleCreated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateSaleAsync(x);
            }
            else
            {
                x.SaleId = SelectedSale.SaleId;
                x.LastEditedTime = DateTimeOffset.UtcNow;
                MessageBox.Show(Constants.SuccessMessageSaleUpdated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateSaleAsync(x);
            }

            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = Constants.ModeLabelAddSale;

            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("Can_ta");
                await _mqttClientService.SubscribeAsync("Can_tieu_ly");

                _mqttClientService.MessageReceived += (s, data) => OnMqttMessageReceived(s, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
                return;
            }

            if (SelectedSale != null)
            {
                CustomerNameTextBox.Text = SelectedSale.CustomerName;
                RFIDCodeTextBox.Text = SelectedSale.RFIDCode;
                WeightTextBox.Text = SelectedSale.ProductWeight?.ToString();
                URLWeightTextBox.Text = SelectedSale.ProductDensity?.ToString();
                DensityTextBox.Text = SelectedSale.ProductDensity?.ToString();
                URLDensityTextBox.Text = SelectedSale.ProductDensity?.ToString();
                StatusTextBox.Text = SelectedSale.Status.ToString();
                ModeLabel.Content = Constants.ModeLabelEditSale;
            }
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

        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.ShowDialog();
            Close();
            mainWindow.Show();
        }

        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                CameraService cameraService = new CameraService();
                Camera newestCamera = await cameraService.GetNewestCameraAsync();

                if (newestCamera == null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Failed to retrieve camera information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                    return;
                }

                if (data.StartsWith("Can_ta:"))
                {
                    string messageContent = data.Substring("Can_ta:".Length);
                    string filePathUrl = await CaptureImageFromCameraAsync(newestCamera, 1);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessMqttMessage(messageContent, "RFID", "Weight", RFIDCodeTextBox, WeightTextBox);
                        ProcessCameraUrlMessage(filePathUrl, RFIDCodeTextBox.Text, URLWeightTextBox);
                        _ = ProcessCustomerMessage(RFIDCodeTextBox.Text);
                        StatusTextBox.Text = "1";
                    });
                }
                else if (data.StartsWith("Can_tieu_ly:"))
                {
                    string messageContent = data.Substring("Can_tieu_ly:".Length);
                    string filePathUrl = await CaptureImageFromCameraAsync(newestCamera, 2);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProcessMqttMessage(messageContent, "RFID", "Density", RFIDCodeTextBox, DensityTextBox);
                        ProcessCameraUrlMessage(filePathUrl, RFIDCodeTextBox.Text, URLDensityTextBox);
                        _ = ProcessCustomerMessage(RFIDCodeTextBox.Text);
                        StatusTextBox.Text = "1";
                    });
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


        private async void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, System.Windows.Controls.TextBox firstTextBox, System.Windows.Controls.TextBox secondTextBox)
        {
            try
            {
                // Split message by :
                string[] messages = messageContent.Split(':');

                if (messages.Length == 2)
                {
                    string rfidValue = messages[0];
                    float currentValue = float.Parse(messages[1]);
                    DateTimeOffset currentTime = DateTimeOffset.Now;

                    Sale sale = await _saleService.GetSaleByRfidAsync(rfidValue);

                    if (sale == null)
                    {
                        sale = new Sale
                        {
                            SaleId = Guid.NewGuid(),
                            RFIDCode = rfidValue,
                            ProductWeight = currentValue,
                            LastEditedTime = currentTime,
                            Status = 1,
                        };

                        // Save the new Sale to the database
                        await _saleService.CreateSaleAsync(sale);
                    }

                    else if (firstKey == "RFID" && secondKey == "Weight")
                    {
                        // Update the sale with the new weight value
                        if (sale.ProductWeight.HasValue)
                        {
                            currentValue += sale.ProductWeight.Value;
                        }

                        sale.ProductWeight = currentValue;

                        await _saleService.UpdateSaleAsync(sale);

                        oldWeightValue = currentValue;
                        firstMessageTime = currentTime;
                        lastRFID = rfidValue;
                    }
                    else if (firstKey == "RFID" && secondKey == "Density")
                    {
                        // Handle the "Can_tieu_ly" topic
                        if (sale.ProductWeight.HasValue && !sale.ProductDensity.HasValue)
                        {
                            sale.ProductDensity = currentValue;

                            await _saleService.UpdateSaleAsync(sale);
                        }
                        else
                        {
                            throw new Exception("Sale with the specified RFID was not found or already has density.");
                        }
                    }

                    // Update UI
                    firstTextBox.Dispatcher.Invoke(() =>
                    {
                        firstTextBox.Text = rfidValue;
                    });

                    secondTextBox.Dispatcher.Invoke(() =>
                    {
                        secondTextBox.Text = currentValue.ToString();
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


        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }

        private async Task ProcessCustomerMessage(string rfidValue)
        {
            try
            {
                Customer customer = await customerService.GetCustomerByRFIDCodeAsync(rfidValue);

                if (customer != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        CustomerNameTextBox.Text = customer.CustomerName;
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("RFID này chưa được đăng ký. Hãy đăng ký RFID trước để nhận thông tin", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing customer message: {ex.Message}");
            }
        }



        private void ProcessCameraUrlMessage(string cameraUrl, string rfidValue, System.Windows.Controls.TextBox urlTextBox)
        {
            try
            {
                DateTimeOffset currentTime = DateTimeOffset.UtcNow;

                // Ensure the UI update is executed on the UI thread
                urlTextBox.Dispatcher.Invoke(() =>
                {
                    // Check if the RFID is the same and the time difference is less than 5 minutes
                    if (lastRFID == rfidValue && firstMessageTime.HasValue)
                    {
                        // Append the new URL to the existing URL
                        if (urlTextBox == URLWeightTextBox)
                        {
                            oldUrl1 = string.IsNullOrEmpty(oldUrl1) ? cameraUrl : $"{oldUrl1}\n{cameraUrl}";
                            urlTextBox.Text = oldUrl1;
                        }
                        else if (urlTextBox == URLDensityTextBox)
                        {
                            oldUrl2 = string.IsNullOrEmpty(oldUrl2) ? cameraUrl : $"{oldUrl2}\n{cameraUrl}";
                            urlTextBox.Text = oldUrl2;
                        }
                    }
                    else
                    {
                        // If it's a new RFID or more than 5 minutes have passed, reset the URLs
                        if (urlTextBox == URLWeightTextBox)
                        {
                            oldUrl1 = cameraUrl;
                            urlTextBox.Text = oldUrl1;
                        }
                        else if (urlTextBox == URLDensityTextBox)
                        {
                            oldUrl2 = cameraUrl;
                            urlTextBox.Text = oldUrl2;
                        }

                        // Update the last RFID and timestamp
                        lastRFID = rfidValue;
                        firstMessageTime = currentTime;
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing camera URL message: {ex.Message}");
            }
        }



        private async Task<string> CaptureImageFromCameraAsync(Camera camera, int cameraIndex)
        {
            string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"{Guid.NewGuid()}_Camera{cameraIndex}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (string.IsNullOrEmpty(cameraUrl))
                {
                    throw new Exception($"Camera URL for Camera {cameraIndex} is invalid.");
                }

                using (var capture = new VideoCapture(cameraUrl))
                {
                    if (!capture.IsOpened)
                    {
                        throw new Exception($"Failed to open Camera {cameraIndex}.");
                    }

                    using (var frame = new Mat())
                    {
                        capture.Read(frame);
                        if (frame.IsEmpty)
                        {
                            throw new Exception($"Failed to capture image from Camera {cameraIndex}.");
                        }

                        // Convert frame to Image<Bgr, byte>
                        Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();

                        // Convert Image<Bgr, byte> to Bitmap
                        Bitmap bitmap = image.ToBitmap();

                        // Save the image to disk
                        bitmap.Save(localFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing image from Camera {cameraIndex}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return localFilePath.ToString();
        }

    }
}

