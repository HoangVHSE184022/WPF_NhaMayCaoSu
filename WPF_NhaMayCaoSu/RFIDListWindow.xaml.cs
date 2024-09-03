using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDListWindow.xaml
    /// </summary>
    public partial class RFIDListWindow : Window
    {
        public Account CurrentAccount { get; set; } = null;
        private RFIDService _service = new();
        public RFIDListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.CurrentAccount = CurrentAccount;
            Close();
            mainWindow.Show();
        }

        private async void LoadDataGrid()
        {
            RFIDDataGrid.ItemsSource = null;
            RFIDDataGrid.Items.Clear();
            RFIDDataGrid.ItemsSource = await _service.GetAllRFIDsAsync(1, 10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
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
            MessageBox.Show("Bạn đang ở cửa sổ Quản lý RFID!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
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

        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.CurrentAccount = CurrentAccount;
            Close();
            mainWindow.Show();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.CurrentAccount = CurrentAccount;
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.SelectedRFID = RFIDDataGrid.SelectedItem as RFID;
            rFIDManagementWindow.CurrentAccount = CurrentAccount;
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
    }
}
