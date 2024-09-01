using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Service.Interfaces;
using System.Diagnostics;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleManagementWindow.xaml
    /// </summary>
    public partial class SaleManagementWindow : System.Windows.Window
    {

        private SaleService _service = new();

        public Sale SelectedSale { get; set; } = null;
        private MqttClientService _mqttClientService = new MqttClientService();

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

            x.ProductWeight = float.Parse(WeightTextBox.Text);
            x.Status = short.Parse(StatusTextBox.Text);
            x.RFIDCode = (RFIDCodeTextBox.Text);

            CameraService cameraService = new CameraService();
            Camera newestCamera = await cameraService.GetNewestCamera();
            string imageFilePath = string.Empty;


            if (SelectedSale == null)
            {
                //imageFilePath = await CaptureImageFromCameraAsync(newestCamera, cameraIndex: 1);

                //// Save image to Firebase and set WeightImageUrl
                //if (!string.IsNullOrEmpty(imageFilePath))
                //{
                //    FirebaseService firebaseService = new();
                //    string firebaseFileName = Path.GetFileName(imageFilePath);
                //    x.WeightImageUrl = await firebaseService.SaveImagePathToDatabaseAsync(imageFilePath, firebaseFileName);
                //}

                //x.CreatedDate = DateTime.Now;
                //x.IsEdited = false;
                //x.LastEditedTime = null;
                //x.ProductDensity = null;
                //x.DensityImageUrl = null;
                MessageBox.Show(Constants.SuccessMessageSaleCreated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateSaleAsync(x);
            }
            else
            {
                //imageFilePath = await CaptureImageFromCameraAsync(newestCamera, cameraIndex: 2);

                //// Save image to Firebase and set DensityImageUrl
                //if (!string.IsNullOrEmpty(imageFilePath))
                //{
                //    FirebaseService firebaseService = new();
                //    string firebaseFileName = Path.GetFileName(imageFilePath);
                //    x.DensityImageUrl = await firebaseService.SaveImagePathToDatabaseAsync(imageFilePath, firebaseFileName);
                //}

                //x.ProductDensity = Double.Parse(DensityTextBox.Text);
                //x.DensityImageUrl = URLDensityTextBox.Text;
                //x.SaleId = SelectedSale.SaleId;
                //x.CreatedDate = SelectedSale.CreatedDate;
                //x.IsEdited = true;
                x.LastEditedTime = DateTime.Now;
                MessageBox.Show(Constants.SuccessMessageSaleUpdated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateSaleAsync(x);
            }

            Close();
        }


        //private async Task<string> CaptureImageFromCameraAsync(Camera camera, int cameraIndex)
        //{
        //    string localFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

        //    try
        //    {
        //        string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
        //        if (!string.IsNullOrEmpty(cameraUrl))
        //        {
        //            using (var capture = new VideoCapture(cameraUrl))
        //            {
        //                if (capture.IsOpened())
        //                {
        //                    using (var frame = new Mat())
        //                    {
        //                        capture.Read(frame);
        //                        if (!frame.Empty())
        //                        {
        //                            BitmapSource bitmapSource = frame.ToBitmapSource();
        //                            using (var stream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
        //                            {
        //                                BitmapEncoder encoder = new JpegBitmapEncoder();
        //                                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        //                                encoder.Save(stream);
        //                            }
        //                            MessageBox.Show(string.Format(Constants.SuccessMessageCapturedFrame, cameraIndex), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show(string.Format(Constants.ErrorMessageCaptureFrameFailed, cameraIndex), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show(string.Format(Constants.ErrorMessageOpenCameraFailed, cameraIndex), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception(string.Format(Constants.ErrorMessageInvalidCameraUrl, cameraIndex));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(string.Format(Constants.ErrorMessageCaptureImage, cameraIndex, ex.Message), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        //        return string.Empty;
        //    }

        //    return localFilePath;
        //}

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = Constants.ModeLabelAddSale;
            await _mqttClientService.SubscribeAsync("Can_ta");
            await _mqttClientService.SubscribeAsync("Can_tieu_ly");

            _mqttClientService.MessageReceived += (s, data) => OnMqttMessageReceived(s, data);

            if (SelectedSale != null)
            {
                RFIDCodeTextBox.Text = SelectedSale.RFIDCode.ToString();
                WeightTextBox.Text = SelectedSale.ProductWeight.ToString();
                //if (SelectedSale.WeightImageUrl != null)
                //{
                //    URLWeightTextBox.Text = SelectedSale.WeightImageUrl.ToString();
                //}
                //DensityTextBox.Text = SelectedSale.ProductDensity.ToString();
                //if (SelectedSale.DensityImageUrl != null)
                //{
                //    URLDensityTextBox.Text = SelectedSale.DensityImageUrl.ToString();
                //}

                StatusTextBox.Text = SelectedSale.Status.ToString();
                ModeLabel.Content = Constants.ModeLabelEditSale;
            }
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("Can_ta:"))
                {
                    ProcessMqttMessage(data.Substring("Can_ta:".Length), "RFID", "Weight", RFIDCodeTextBox, WeightTextBox);
                }
                else if (data.StartsWith("Can_tieu_ly:"))
                {
                    ProcessMqttMessage(data.Substring("Can_tieu_ly:".Length), "RFID", "Density", RFIDCodeTextBox, DensityTextBox);
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

        private void ProcessMqttMessage(string messageContent, string firstKey, string secondKey, System.Windows.Controls.TextBox firstTextBox, System.Windows.Controls.TextBox secondTextBox)
        {
            try
            {
                string[] messages = messageContent.Split(';');

                string firstValue = null;
                string secondValue = null;

                foreach (string message in messages)
                {
                    if (message.StartsWith(firstKey + ":"))
                    {
                        firstValue = message.Substring((firstKey + ":").Length);
                    }
                    else if (message.StartsWith(secondKey + ":"))
                    {
                        secondValue = message.Substring((secondKey + ":").Length);
                    }
                }
                if (!string.IsNullOrEmpty(firstValue))
                {
                    firstTextBox.Dispatcher.Invoke(() =>
                    {
                        firstTextBox.Text = firstValue;
                    });
                }
                else
                {
                    Debug.WriteLine($"Failed to parse {firstKey}.");
                }

                // Cập nhật TextBox cho giá trị thứ hai
                if (!string.IsNullOrEmpty(secondValue))
                {
                    secondTextBox.Dispatcher.Invoke(() =>
                    {
                        secondTextBox.Text = secondValue;
                    });
                }
                else
                {
                    Debug.WriteLine($"Failed to parse {secondKey}.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message content: {ex.Message}");
            }
        }


    }
}

