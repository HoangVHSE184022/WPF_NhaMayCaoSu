using Serilog;
using System.Windows;
using System.Windows.Input;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool isForcedShut = true;
        private readonly IAccountService _accountService = new AccountService();
        public event Action<Account> LoginSucceeded;

        public LoginWindow()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
            UsernameTextBox.KeyDown += TextBox_KeyDown;
            PasswordTextBox.KeyDown += TextBox_KeyDown;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;


            // Call the login service
            try
            {
                Account account = await _accountService.LoginAsync(username, password);
                if (account == null)
                {
                    MessageBox.Show(Constants.ErrorMessageInvalidLogin, Constants.ErrorTitleLoginFailed, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    LoginSucceeded?.Invoke(account);
                    //MainControl control = new();
                    //control.CurrentAccount = account;
                    //control.Show();
                    isForcedShut = false;
                    Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.Error(ex, "Đã xảy ra lỗi");
                return;
            }
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Hành động này sẽ đóng ứng dụng, bạn chắc chứ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.ShowDialog();

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, new RoutedEventArgs());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(isForcedShut)
            {
                App.Current.Shutdown();
            }

        }
    }
}
