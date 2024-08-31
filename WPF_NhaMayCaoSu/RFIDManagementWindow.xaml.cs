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
using WPF_NhaMayCaoSu.Core.Utils;

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
        public Account CurrentAccount { get; set; } = null;
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
                CustomerComboBox.SelectedValue = SelectedRFID.CustomerId.ToString();
                ModeLabel.Content = "Chỉnh sửa RFID";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(RFIDCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpDateDatePicker.Text) ||
                string.IsNullOrWhiteSpace(StatusTextBox.Text))
            {
                MessageBox.Show(Constants.ErrorMessageMissingFields, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            // Validate Status (ensure it's either 0 or 1)
            if (!short.TryParse(StatusTextBox.Text, out short status) || (status != 0 && status != 1))
            {
                MessageBox.Show(Constants.ErrorMessageInvalidStatus, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Proceed to create or update the customer
            RFID rFID = new()
            {
                RFIDCode = RFIDCodeTextBox.Text,
                ExpirationDate = DateTime.Parse(ExpDateDatePicker.Text),
                Status = status,
                RFID_Id = SelectedRFID?.RFID_Id ?? Guid.NewGuid(),
                CustomerId = Guid.Parse(CustomerComboBox.SelectedValue.ToString()),
            };

            if (SelectedRFID == null)
            {
                await _service.AddRFIDAsync(rFID);
                MessageBox.Show("Đã tạo thành công!", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _service.UpdateRFIDAsync(rFID);
                MessageBox.Show("Chỉnh sửa thành công!", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            rFIDListWindow.ShowDialog();
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

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
    }
}
