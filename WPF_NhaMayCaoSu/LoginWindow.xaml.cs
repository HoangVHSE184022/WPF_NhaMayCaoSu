using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Content;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService = new();
        public event Action<Account> LoginSucceeded;

        public LoginWindow()
        {
            InitializeComponent();
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
                    Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            Close();
            accountManagementWindow.Show();

        }
    }
}
