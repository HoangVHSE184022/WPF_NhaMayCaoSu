using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RoleListWindow.xaml
    /// </summary>
    public partial class RoleListWindow : Window
    {

        public Account CurrentAccount { get; set; } = null;
        private IRoleService _service = new RoleService();
        public RoleListWindow()
        {
            InitializeComponent();
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
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
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
            MainWindow window = new MainWindow();
            window.CurrentAccount = CurrentAccount;
            Close();
            window.Show();
        }

        private async void LoadDataGrid()
        {
            RoleDataGrid.ItemsSource = null;
            RoleDataGrid.Items.Clear();
            RoleDataGrid.ItemsSource = await _service.GetAllRolesAsync(1, 10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                AddRoleButton.Visibility = Visibility.Collapsed;
                EditRoleButton1.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private void AddRoleButton_Click(object sender, RoutedEventArgs e)
        {
            RoleManagementWindow roleManagementWindow = new RoleManagementWindow();
            roleManagementWindow.CurrentAccount = CurrentAccount;
            roleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditRoleButton_Click(object sender, RoutedEventArgs e)
        {
            Role selected = RoleDataGrid.SelectedItem as Role;

            if (selected == null)
            {
                MessageBox.Show("Chọn 1 vai trò trước khi update!", "Chọn vai trò!", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            RoleManagementWindow roleManagementWindow = new RoleManagementWindow();
            roleManagementWindow.CurrentAccount = CurrentAccount;
            roleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đang ở cửa sổ Quản lý vai trò!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
