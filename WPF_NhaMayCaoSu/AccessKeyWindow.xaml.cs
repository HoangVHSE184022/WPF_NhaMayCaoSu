using System.Windows;
using WPF_NhaMayCaoSu.Service.Trial;

namespace WPF_NhaMayCaoSu
{
    public partial class AccessKeyWindow : Window
    {
        private readonly TrialManager _trialManager;
        private readonly KeyManager _keyManager;

        public AccessKeyWindow()
        {
            InitializeComponent();
            _trialManager = new TrialManager();
            _keyManager = new KeyManager();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check activation status and update the status label accordingly
            if (_keyManager.IsActivated())
            {
                StatusLabel.Content = "Trạng thái: Đã kích hoạt";
                LicenseKeyLabel.Visibility = Visibility.Visible;
                LicenseKeyLabel.Content = _keyManager.GetStoredKey(); // Display stored key
            }
            else
            {
                StatusLabel.Content = "Trạng thái: Chưa kích hoạt";
                LicenseKeyLabel.Visibility = Visibility.Collapsed;
            }

            // Show the remaining trial days in the expiration label
            ExpLabel.Content = $"Thời hạn dùng thử: {_trialManager.GetRemainingTrialDays()} ngày";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredKey = KeyTextBox.Text;

            if (_keyManager.ValidateKey(enteredKey))
            {
                _keyManager.SaveActivation(enteredKey); // Save the activation key
                MessageBox.Show("Kích hoạt thành công!");
                StatusLabel.Content = "Trạng thái: Đã kích hoạt";
                LicenseKeyLabel.Visibility = Visibility.Visible;
                LicenseKeyLabel.Content = enteredKey; // Display the entered key
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Mã kích hoạt không hợp lệ. Vui lòng thử lại.");
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (_trialManager.IsTrialExpired())
            {
                MessageBox.Show("Thời gian dùng thử đã hết. Vui lòng nhập mã kích hoạt.");
            }
            else
            {
                this.DialogResult = true;
                Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Close the application
        }
    }
}
