using System.Diagnostics;
using System.Windows;
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
        private MqttClientService _mqttClientService = new MqttClientService();
        public Account CurrentAccount { get; set; } = null;

        private Customer _customer;
        public RFIDManagementWindow()
        {
            InitializeComponent();
        }

        public RFIDManagementWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;
        }

        public RFIDManagementWindow(string rfidCode)
        {
            InitializeComponent();
            RFIDCodeTextBox.Text = rfidCode;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm RFID mới";

            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("CreateRFID");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
                return;
            }

            CustomerComboBox.ItemsSource = null;
            CustomerComboBox.ItemsSource = await _customerService.GetAllCustomers(1, 100);

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

            if (_customer != null)
            {
                ExpDateDatePicker.Text = DateTimeOffset.UtcNow.AddDays(30).ToString("dd/MM/yyyy");
                StatusTextBox.Text = "1";
                CustomerComboBox.SelectedValue = _customer.CustomerId.ToString();
            }

            _mqttClientService.MessageReceived += OnMqttMessageReceived;
        }


        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("CreateRFID:"))
                {
                    string rfidString = data.Substring("CreateRFID:".Length);

                    if (!string.IsNullOrEmpty(rfidString))
                    {
                        RFIDCodeTextBox.Dispatcher.Invoke(() =>
                        {
                            RFIDCodeTextBox.Text = rfidString;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Failed to parse RFID number.");
                    }
                }
                else
                {
                    Debug.WriteLine("Unexpected message format.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
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
                rFID.CreatedDate = DateTimeOffset.UtcNow;
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
            Close();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
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
            MainWindow window = new MainWindow();
            window.CurrentAccount = CurrentAccount;
            Close();
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
            Close();
            roleListWindow.ShowDialog();
        }
    }
}
