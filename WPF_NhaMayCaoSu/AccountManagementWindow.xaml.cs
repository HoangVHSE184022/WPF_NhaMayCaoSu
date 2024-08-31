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
        public AccountManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string accountName = AccountNameTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;
            if(accountName.IsNullOrEmpty() || username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                MessageBox.Show(Constants.ErrorMessageMissingInfo, Constants.TitlePleaseTryAgain, MessageBoxButton.OK, MessageBoxImage.Warning);
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
            customerListWindow.ShowDialog();
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

        }
    }
}
