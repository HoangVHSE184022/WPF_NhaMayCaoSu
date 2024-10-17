using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF_NhaMayCaoSu.OTPService;

namespace WPF_NhaMayCaoSu
{
    public partial class AccessKeyWindow : Window
    {
        private OTPServices _otpService;
        private RegistryHelper _registryHelper;
        private string _secretKey;
        private bool _closeWithoutExit = false;
    
        public AccessKeyWindow()
        {
            InitializeComponent();
            _otpService = new OTPServices("CaoSuApp", "ActivateKey");
            _registryHelper = new RegistryHelper();

            LoadOrGenerateSecretKey();
            CheckDemoStatus();
        }


        private void LoadOrGenerateSecretKey()
        {
            _secretKey = _otpService.GenerateSecretKey();

            GenerateQRCode();
        }

        private void GenerateQRCode()
        {
            string googleAuthUri = _otpService.GenerateGoogleAuthUri(_secretKey);
            Bitmap qrCodeBitmap = _otpService.GenerateQRCode(googleAuthUri);
            QR.Source = ConvertBitmapToImageSource(qrCodeBitmap);
        }

        private void CheckDemoStatus()
        {
            // Debugging
            Console.WriteLine("Checking demo status...");
            ConfirmButton.IsEnabled = true;
            KeyTextBox.IsEnabled = true;

            if (_registryHelper.IsUnlocked())
            {
                StatusLabel.Content = "Ứng dụng đã được mở khóa!";
                ConfirmButton.IsEnabled = false;
                KeyTextBox.IsEnabled = false;
                ExpLabel.Content = string.Empty;
                Close();
                return;
            }

            DateTime? demoStartDate = _registryHelper.GetDemoStartDate();
            Console.WriteLine($"Demo start date: {demoStartDate}");

            if (demoStartDate == null)
            {
                _registryHelper.SetDemoStartDate(DateTime.Now);
                ExpLabel.Content = "Thời hạn dùng thử: 30 ngày còn lại";
                ConfirmButton.IsEnabled = true;
                KeyTextBox.IsEnabled = true;
                return;
            }
            else
            {
                TimeSpan elapsedTime = DateTime.Now - demoStartDate.Value;
                int remainingDays = 30 - (int)elapsedTime.TotalDays;

                // Debugging
                Console.WriteLine($"Remaining days: {remainingDays}");

                if (remainingDays <= 0)
                {
                    StatusLabel.Content = "Thời hạn dùng thử đã hết!";
                    ExpLabel.Content = string.Empty;
                    ContinueButton.IsEnabled = false;
                    Console.WriteLine("Trial period has expired.");
                }
                else
                {
                    ExpLabel.Content = $"Thời hạn dùng thử: {remainingDays} ngày còn lại";
                    ConfirmButton.IsEnabled = true;
                    KeyTextBox.IsEnabled = true;
                    Console.WriteLine("Trial period active. Inputs enabled.");
                }
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string userInputCode = KeyTextBox.Text.Trim();

            if (string.IsNullOrEmpty(_secretKey))
            {
                MessageBox.Show("Secret key is not initialized. Please restart the application.");
                return;
            }

            bool isValid = _otpService.VerifyTotp(_secretKey, userInputCode);

            if (isValid)
            {
                _registryHelper.SetUnlocked();
                MessageBox.Show("Ứng dụng đã được mở khóa vĩnh viễn!");
                StatusLabel.Content = "Ứng dụng đã được mở khóa!";
                _closeWithoutExit = true; // Set the flag to true
                Close();
            }
            else
            {
                MessageBox.Show("Mã OTP không hợp lệ, vui lòng thử lại.");
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? demoStartDate = _registryHelper.GetDemoStartDate();
            if (demoStartDate == null || DateTime.Now - demoStartDate.Value <= TimeSpan.FromDays(30))
            {
                _closeWithoutExit = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Thời hạn dùng thử đã hết!");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private BitmapImage ConvertBitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_closeWithoutExit)
            {
                e.Cancel = true;
                _closeWithoutExit = false;
                Close();
            }
            else
            {
                App.Current.Shutdown();
            }
        }
    }
}
