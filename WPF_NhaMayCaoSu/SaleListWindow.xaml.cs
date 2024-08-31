using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {

        private SaleService _service = new();

        public SaleListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {

            Sale selected = SaleDataGrid.SelectedItem as Sale;

            if (selected == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectSale, Constants.ErrorTitleSelectSale, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.SelectedSale = selected;
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private async void LoadDataGrid()
        {
            SaleDataGrid.ItemsSource = null;
            SaleDataGrid.Items.Clear();
            SaleDataGrid.ItemsSource = await _service.GetAllSaleAsync(1, 10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.ShowDialog();
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
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
