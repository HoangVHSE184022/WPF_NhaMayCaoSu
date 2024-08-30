using Microsoft.IdentityModel.Tokens;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for AccountManagementWindow.xaml
    /// </summary>
    public partial class AccountManagementWindow : Window
    {
        private readonly AccountService _accountService = new();
        public AccountManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string accountName = AccountNameTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;
            if(accountName.IsNullOrEmpty() || username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                MessageBox.Show("Xin hãy nhập tất cả thông tin","Please try again", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Account account = new();
            account.AccountName = accountName;
            account.Username = username;
            account.Password = password;
            await _accountService.RegisterAsync(account);
            AccountNameTextBox.Text = "";
            UsernameTextBox.Text = "";
            PasswordTextBox.Password = "";
            MessageBox.Show("Tạo tài khoản thành công", "Đăng ký thành công", MessageBoxButton.OK);
            LoginWindow login = new();
            login.Show();
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new();
            login.Show();
            Close();
        }

    }
}
