using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF_NhaMayCaoSu.OTPService;
using WPF_NhaMayCaoSu.Service.Trial;

namespace WPF_NhaMayCaoSu
{
    public partial class AccessKeyWindow : Window
    {
        private OTPServices _otpService;
        private RegistryHelper _registryHelper;
        private string _secretKey;

        public AccessKeyWindow()
        {
            InitializeComponent();
            _otpService = new OTPServices("CaoSuApp", "quanghuy01062004@gmail.com");
            _registryHelper = new RegistryHelper();

            CheckDemoStatus();
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
                _registryHelper.SetDemoStartDate(DateTime.Now); // Set the current date
                ExpLabel.Content = "Thời hạn dùng thử: 30 ngày còn lại"; // Initial message for new users
                ConfirmButton.IsEnabled = true; // Allow user to input key
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
                    ExpLabel.Content = string.Empty; // No expiration message
                    ContinueButton.IsEnabled = false; // Disable button if expired
                    Console.WriteLine("Trial period has expired.");
                }
                else
                {
                    ExpLabel.Content = $"Thời hạn dùng thử: {remainingDays} ngày còn lại"; // Update the label with remaining days
                    ConfirmButton.IsEnabled = true; // Allow input if not expired
                    KeyTextBox.IsEnabled = true; // Allow input if not expired
                    Console.WriteLine("Trial period active. Inputs enabled.");
                }
            }
        }



        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string userInputCode = KeyTextBox.Text;

            _secretKey = "CaoSuAmazingTechActivationKey";

            bool isValid = _otpService.VerifyTotp(_secretKey, userInputCode);

            if (isValid)
            {
                _registryHelper.SetUnlocked();
                MessageBox.Show("Ứng dụng đã được mở khóa vĩnh viễn!");
                StatusLabel.Content = "Ứng dụng đã được mở khóa!";
                Close();
            }
            else
            {
                MessageBox.Show("Mã OTP không hợp lệ, vui lòng thử lại.");
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            // Allow the user to continue using the trial if the trial period has not expired
            DateTime? demoStartDate = _registryHelper.GetDemoStartDate();
            if (demoStartDate == null || DateTime.Now - demoStartDate.Value <= TimeSpan.FromDays(30))
            {
                this.Close(); // Allow the app to continue
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _secretKey = _otpService.GenerateSecretKey();
            string googleAuthUri = _otpService.GenerateGoogleAuthUri(_secretKey);

            Bitmap qrCodeBitmap = _otpService.GenerateQRCode(googleAuthUri);

            QR.Source = ConvertBitmapToImageSource(qrCodeBitmap);
        }
        private BitmapImage ConvertBitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp); // Save bitmap to memory stream
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory; // Load stream to BitmapImage
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }

}
