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
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDManagementWindow.xaml
    /// </summary>
    public partial class RFIDManagementWindow : Window
    {

        private RFIDService _service = new();
        private CustomerService _customerService = new();
        public RFID SelectedRFID { get; set; } = null;
        public RFIDManagementWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm RFID mới";

            CustomerComboBox.ItemsSource = await _customerService.GetAllCustomers(1,100);

            CustomerComboBox.DisplayMemberPath = "CustomerName";
            CustomerComboBox.SelectedValuePath = "CustomerId";

            if (SelectedRFID != null)
            {
                RFIDCodeTextBox.Text = SelectedRFID.RFIDCode.ToString();
                ExpDateDatePicker.Text = SelectedRFID.ExpirationDate.ToString();
                StatusTextBox.Text = SelectedRFID.Status.ToString();
                ModeLabel.Content = "Chỉnh sửa RFID";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

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
            MainWindow window = new MainWindow();
            window.Show();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
    }
}
