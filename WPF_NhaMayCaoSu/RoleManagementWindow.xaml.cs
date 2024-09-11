using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RoleManagementWindow.xaml
    /// </summary>
    public partial class RoleManagementWindow : Window
    {
        public Account CurrentAccount { get; set; } = null;

        public Role SelectedRole { get; set; } = null;

        private IRoleService _service = new RoleService();

        public RoleManagementWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm vai trò mới";

            if (SelectedRole != null)
            {
                RoleTextBox.Text = SelectedRole.RoleName.ToString();
                ModeLabel.Content = "Chỉnh sửa vai trò";
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoleTextBox.Text))
            {
                MessageBox.Show("Tên vai trò không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Role role = new()
            {
                RoleName = RoleTextBox.Text,
                RoleId = SelectedRole?.RoleId ?? Guid.NewGuid()
            };

            if (SelectedRole == null)
            {
                await _service.CreateRoleAsync(role);
                MessageBox.Show("Đã tạo thành công!", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _service.UpdateRoleAsync(role);
                MessageBox.Show("Chỉnh sửa thành công!", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }


        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
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
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
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
            MainWindow mainWindow = new();
            mainWindow.ShowDialog();
            Close();
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            Close();
            roleListWindow.ShowDialog();
        }
    }
}