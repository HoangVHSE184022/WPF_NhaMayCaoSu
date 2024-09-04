using Microsoft.IdentityModel.Tokens;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for AccountManagementWindow.xaml
    /// </summary>
    public partial class AccountManagementWindow : Window
    {
        private readonly AccountService _accountService = new();
        private readonly RoleService _roleService = new();
        public Account CurrentAccount { get; set; } = null;
        public AccountManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new();
            window.CurrentAccount = CurrentAccount;
            Close();
            window.Show();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string accountName = AccountNameTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;
            Guid roleId = Guid.Parse(RoleComboBox.SelectedValue.ToString());
            if (accountName.IsNullOrEmpty() || username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                MessageBox.Show(Constants.ErrorMessageMissingInfo, Constants.TitlePleaseTryAgain, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PasswordTextBox.Password != ConfirmPasswordTextBox.Password)
            {
                MessageBox.Show("Sai mật khẩu xác nhận", "Xác nhận mật khẩu thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Account account = new();
            account.AccountName = accountName;
            account.Username = username;
            account.Password = password;
            account.RoleId = roleId;
            await _accountService.RegisterAsync(account);
            MessageBox.Show(Constants.SuccessMessageCreateAccount, Constants.TitleRegisterSuccess, MessageBoxButton.OK);
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

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
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
            MessageBox.Show("Bạn đang ở cửa sổ này rồi!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
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
            MainWindow window = new();
            window.CurrentAccount = CurrentAccount;
            Close();
            window.Show();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Đăng ký tài khoản";

            RoleComboBox.ItemsSource = await _roleService.GetAllRolesAsync(1, 100);

            RoleComboBox.DisplayMemberPath = "RoleName";
            RoleComboBox.SelectedValuePath = "RoleId";

            //RoleComboBox.IsEnabled = false;

            if (CurrentAccount != null)
            {
                ModeLabel.Content = "Cập nhật tài khoản";
                AccountNameTextBox.Text = CurrentAccount.AccountName;
                UsernameTextBox.Text = CurrentAccount.Username;
                LoginButton.Content = "";
                LoginButton.IsEnabled = false;
            }
        }
    }
}
