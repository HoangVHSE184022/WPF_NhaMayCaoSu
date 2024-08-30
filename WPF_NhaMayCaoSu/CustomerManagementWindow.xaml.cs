using System;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerManagementWindow.xaml
    /// </summary>
    public partial class CustomerManagementWindow : Window
    {
        private CustomerService _service = new();

        public Customer SelectedCustomer { get; set; } = null;

        public CustomerManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(RFIDCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(StatusTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate RFID Code (ensure it's a valid number)
            if (!long.TryParse(RFIDCodeTextBox.Text, out long rfidCode))
            {
                MessageBox.Show("RFID Code phải là số hợp lệ!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Status (ensure it's either 0 or 1)
            if (!short.TryParse(StatusTextBox.Text, out short status) || (status != 0 && status != 1))
            {
                MessageBox.Show("Status phải là 0 (inactive) hoặc 1 (active)!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Proceed to create or update the customer
            Customer customer = new()
            {
                CustomerName = AccountNameTextBox.Text,
                RFIDCode = rfidCode,
                Status = status,
                CreatedDate = SelectedCustomer == null ? DateTime.Now : SelectedCustomer.CreatedDate,
                ExpirationDate = SelectedCustomer == null ? DateTime.Now.AddDays(1) : SelectedCustomer.ExpirationDate,
                CustomerId = SelectedCustomer?.CustomerId ?? Guid.NewGuid()
            };

            if (SelectedCustomer == null)
            {
                await _service.CreateCustomer(customer);
                MessageBox.Show($"Khách hàng {customer.CustomerName} đã được tạo thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _service.UpdateCustomer(customer);
                MessageBox.Show($"Khách hàng {customer.CustomerName} đã được cập nhật thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm khách hàng mới";

            if (SelectedCustomer != null)
            {
                AccountNameTextBox.Text = SelectedCustomer.CustomerName;
                RFIDCodeTextBox.Text = SelectedCustomer.RFIDCode.ToString();
                StatusTextBox.Text = SelectedCustomer.Status.ToString();
                ModeLabel.Content = "Chỉnh sửa khách hàng";
            }
        }
    }
}
