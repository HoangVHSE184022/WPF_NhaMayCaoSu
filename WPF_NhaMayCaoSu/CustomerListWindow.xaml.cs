using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

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
            this.Close();
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
                MessageBox.Show("Please select a Customer to update!", "Select One", MessageBoxButton.OK, MessageBoxImage.Stop);
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
    }
}
