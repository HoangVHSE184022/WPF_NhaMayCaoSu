using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RoleListWindow.xaml
    /// </summary>
    public partial class RoleListWindow : Window
    {

        public Account CurrentAccount { get; set; } = null;
        private RoleService _service = new();
        public RoleListWindow()
        {
            InitializeComponent();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            customerListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.CurrentAccount = CurrentAccount;
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            rFIDListWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.CurrentAccount = CurrentAccount;
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
            LoadDataGrid();
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
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
