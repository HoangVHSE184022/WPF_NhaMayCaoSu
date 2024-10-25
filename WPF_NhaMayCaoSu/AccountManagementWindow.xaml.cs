using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for AccountManagementWindow.xaml
    /// </summary>
    public partial class AccountManagementWindow : Window
    {
        private readonly IAccountService _accountService = new AccountService();
        private readonly IRoleService _roleService = new RoleService();
        public Account CurrentAccount { get; set; } = null;
        public Account SelectedAccount { get; set; } = null;
        public AccountManagementWindow()
        {
            LoggingHelper.ConfigureLogger();
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
            Guid roleId;

            // Validate that all necessary fields are filled
            if (accountName.IsNullOrEmpty() || username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                MessageBox.Show(Constants.ErrorMessageMissingInfo, Constants.TitlePleaseTryAgain, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate password confirmation
            if (PasswordTextBox.Password != ConfirmPasswordTextBox.Password)
            {
                MessageBox.Show("Sai mật khẩu xác nhận", "Xác nhận mật khẩu thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If CurrentAccount is not null (Edit mode)
            if (CurrentAccount != null)
            {

                // If the role is "Admin", allow them to choose a new role
                if (CurrentAccount.Role?.RoleName == "Admin")
                {
                    if (RoleComboBox.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng chọn vai trò.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    roleId = Guid.Parse(RoleComboBox.SelectedValue.ToString());
                }
                else
                {
                    // If the user is not an Admin, keep their current role
                    roleId = CurrentAccount.RoleId;
                }

                // Create Account object for update
                Account updatedAccount = new()
                {
                    AccountId = CurrentAccount.AccountId,
                    AccountName = accountName,
                    Username = username,
                    Password = password,
                    RoleId = roleId
                };

                await _accountService.UpdateAccountAsync(updatedAccount);
                MessageBox.Show("Cập nhật thành công!", "Cập nhật thành công!", MessageBoxButton.OK);
            }
            else  // If CurrentAccount is null (Register mode)
            {

                // Check if a "User" role exists, create one if it doesn't
                Role userRole = await _roleService.GetRoleByNameAsync("User");
                if (userRole == null)
                {
                    userRole = new Role
                    {
                        RoleId = Guid.NewGuid(),
                        RoleName = "User"
                    };
                    await _roleService.CreateRoleAsync(userRole);
                }

                // Set roleId to "User"
                roleId = userRole.RoleId;

                // Create new Account object
                Account account = new()
                {
                    AccountName = accountName,
                    Username = username,
                    Password = password,
                    RoleId = roleId
                };

                // Check if username is unique
                Account existingAccount = await _accountService.GetAccountByUsernameAsync(username);
                if (existingAccount != null)
                {
                    MessageBox.Show("Tên người dùng đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Register the new account
                try
                {
                    await _accountService.RegisterAsync(account);
                    MessageBox.Show("Đăng ký thành công!", "Đăng ký thành công", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đăng ký tài khoản!", "Lỗi!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Log.Error(ex, $"Lỗi khi đăng ký tài khoản!");
                }

                // Redirect to login window
                //LoginWindow login = new();
                //login.Show();
                Close();
            }


            Close();
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
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
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
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
            SaveButton.Content = "Đăng ký";

            //RoleComboBox.IsEnabled = false;
            //if (CurrentAccount?.Role?.RoleName != "Admin")

            //{
            //    RoleComboBox.Visibility = Visibility.Collapsed;
            //    Role.Visibility = Visibility.Collapsed;
            //}

            if (SelectedAccount != null)
            {
                SaveButton.Content = "Cập nhật";
                ModeLabel.Content = "Cập nhật tài khoản";
                AccountNameTextBox.Text = SelectedAccount.AccountName;
                UsernameTextBox.Text = SelectedAccount.Username;
                RoleComboBox.SelectedValue = SelectedAccount.RoleId;
            }

        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }
    }
}
