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
            Customer x = new();
            x.CustomerName = AccountNameTextBox.Text;
            x.RFIDCode = long.Parse(RFIDCodeTextBox.Text);
            x.Status = short.Parse(StatusTextBox.Text);

            if (SelectedCustomer == null)
            {
                x.CreatedDate = DateTime.Now;
                x.ExpirationDate = DateTime.Now.AddDays(1);

                MessageBox.Show($"Created {AccountNameTextBox.Text}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateCustomer(x);
            }
            else
            {
                MessageBox.Show($"Updated {SelectedCustomer.CustomerName}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateCustomer(x);
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
