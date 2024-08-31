using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {

        private CustomerService _service = new();

        public CustomerListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditCustomerButton1_Click(object sender, RoutedEventArgs e)
        {

            Customer selected = CustomerDataGrid.SelectedItem as Customer;

            if (selected == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectCustomer, Constants.ErrorTitleSelectCustomer, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.SelectedCustomer = selected;
            customerManagementWindow.ShowDialog();
            LoadDataGrid();
        }


        private async void LoadDataGrid()
        {
            CustomerDataGrid.ItemsSource = null;
            CustomerDataGrid.Items.Clear();
            CustomerDataGrid.ItemsSource = await _service.GetAllCustomers(1, 10);
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

        }
    }
}
