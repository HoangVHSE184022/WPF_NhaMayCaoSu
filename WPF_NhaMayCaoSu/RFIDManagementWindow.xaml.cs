using System.Diagnostics;
using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using Serilog;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDManagementWindow.xaml
    /// </summary>
    public partial class RFIDManagementWindow : Window
    {

        private IRFIDService _service = new RFIDService();
        private ICustomerService _customerService = new CustomerService();
        public RFID SelectedRFID { get; set; } = null;
        private MqttClientService _mqttClientService = new MqttClientService();
        private IBoardService _boardService = new BoardService();
        private bool isUnlimited = false;
        public Account CurrentAccount { get; set; } = null;

        private Customer _customer;
        public RFIDManagementWindow()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
        }

        public RFIDManagementWindow(Customer customer)
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
            _customer = customer;
        }

        public RFIDManagementWindow(string rfidCode)
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
            RFIDCodeTextBox.Text = rfidCode;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm RFID mới";
            StatusTextBox.Text = "1";

            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("+/sendRFID");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Không thể kết nối đến máy chủ MQTT");

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
                ExpDateDatePicker.Text = DateTime.UtcNow.AddDays(30).ToString();
                StatusTextBox.Text = "1";
                CustomerComboBox.SelectedValue = _customer.CustomerId.ToString();
            }

            _mqttClientService.MessageReceived += OnMqttMessageReceived;
        }


        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("sendRFID-"))
                {
                    string[] parts = data.Split('-');
                    string macAddress = parts[1];
                    string rfidString = parts[2];

                    Board board = await _boardService.GetBoardByMacAddressAsync(macAddress);
                    if (board == null)
                    {
                        MessageBox.Show("Dữ liệu được gửi từ board không xác định!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!string.IsNullOrEmpty(rfidString))
                    {
                        RFIDCodeTextBox.Dispatcher.Invoke(() =>
                        {
                            RFIDCodeTextBox.Text = rfidString;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Lỗi khi sử dụng mã RFID.");
                    }
                }
                else
                {
                    Debug.WriteLine("Sai kiểu dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
                Log.Error(ex, "Error processing message");
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RFIDCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ExpDateDatePicker.Text) ||
                string.IsNullOrWhiteSpace(StatusTextBox.Text))
            {
                MessageBox.Show(Constants.ErrorMessageMissingFields, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            RFID thisRFID = await _service.GetRFIDByRFIDCodeAsync(RFIDCodeTextBox.Text);
            if (thisRFID != null)
            {
                MessageBox.Show("RFID đã được khởi tạo", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isUnlimited && (string.IsNullOrWhiteSpace(ExpDateDatePicker.Text) || DateTime.Parse(ExpDateDatePicker.Text) < DateTime.Today))
            {
                MessageBox.Show("Ngày hết hạn không được là ngày trong quá khứ.", "Lỗi xác thực", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                ExpirationDate = isUnlimited ? DateTime.UtcNow.AddYears(100) : DateTime.Parse(ExpDateDatePicker.Text),
                Status = status,
                RFID_Id = SelectedRFID?.RFID_Id ?? Guid.NewGuid(),
                CustomerId = Guid.Parse(CustomerComboBox.SelectedValue.ToString()),
            };

            if (SelectedRFID == null)
            {
                rFID.CreatedDate = DateTime.UtcNow;
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

        private void UnlimitedButton_Click(object sender, RoutedEventArgs e)
        {
            if (isUnlimited)
            {
                // Nếu đang là "Vô thời hạn", hiện lại DatePicker và bỏ giá trị vô thời hạn
                ExpDateDatePicker.Visibility = Visibility.Visible;
                ExpDateDatePicker.SelectedDate = null;
                UnlimitedButton.Content = "Vô thời hạn";
                isUnlimited = false;
            }
            else
            {
                // Nếu đang chọn ngày, ẩn DatePicker và set giá trị "Vô thời hạn"
                ExpDateDatePicker.Visibility = Visibility.Collapsed;
                ExpDateDatePicker.SelectedDate = DateTime.UtcNow.AddYears(100);
                UnlimitedButton.Content = "Chỉnh ngày";
                isUnlimited = true;
            }
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
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
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
