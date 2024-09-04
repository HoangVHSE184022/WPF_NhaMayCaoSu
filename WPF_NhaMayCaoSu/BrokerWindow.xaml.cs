using System.Windows;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BrokerWindow.xaml
    /// </summary>
    public partial class BrokerWindow : Window
    {
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        public Account CurrentAccount { get; set; } = null;


        public BrokerWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
            _mqttServerService.BrokerStatusChanged += (sender, e) => UpdateBrokerUI();
            UpdateBrokerUI();
            _mqttServerService.ClientsChanged += MqttService_ClientsChanged;
            _mqttClientService = new MqttClientService();
        }

        private void UpdateBrokerUI()
        {
            if (MqttServerService.IsBrokerRunning)
            {
                ServerStatusLabel.Content = Constants.StatusOnline;
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                PortconnecttionLabel.Content = "1883";
                string localIpAddress = GetLocalIpAddress();
                IPconnecttionSmallLabel.Content = $"Local IP: {localIpAddress}";
                IPconnecttionLabel.Content = $"{localIpAddress}";
            }
            else
            {
                ServerStatusLabel.Content = Constants.StatusOffline;
                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
                PortconnecttionLabel.Content = "Không có kết nối";
                IPconnecttionSmallLabel.Content = "Không có kết nối";
                IPconnecttionLabel.Content = "Không có kết nối";
            }
        }

        //Get local IP of server
        private string GetLocalIpAddress()
        {
            System.Net.IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in ipAddresses)
            {
                // Check for IPv4 addresses
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "N/A";
        }

        private async void StartBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MqttServerService.IsBrokerRunning)
                {
                    MessageBox.Show("The broker is already running.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                // Start the MQTT broker
                await _mqttServerService.StartBrokerAsync();

                // Update the ServerStatusLabel to "Online"
                ServerStatusLabel.Content = Constants.StatusOnline;
                PortconnecttionLabel.Content = "1883";
                string localIpAddress = GetLocalIpAddress();
                IPconnecttionSmallLabel.Content = $"Local IP: {localIpAddress}";
                IPconnecttionLabel.Content = $"{localIpAddress}";


                await _mqttClientService.ConnectAsync();

                // Disable the Start button when the server is online
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Show the error in a MessageBox
                MessageBox.Show($"{Constants.ErrorMessageBrokerStart}: {ex}", Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                // Update the ServerStatusLabel to indicate an error
                ServerStatusLabel.Content = Constants.StatusError;
            }
        }

        private async void StopBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop the MQTT broker
                await _mqttServerService.StopBrokerAsync();

                // Update the ServerStatusLabel to "Offline"
                ServerStatusLabel.Content = Constants.StatusOnline;
                PortconnecttionLabel.Content = "Không có kết nối";

                await _mqttClientService.CloseConnectionAsync();

                // Enable the Start button when the server is offline
                StartButton.IsEnabled = true;

                StopButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Constants.ErrorMessageBrokerStop}: {ex}", Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                ServerStatusLabel.Content = Constants.StatusError;
            }
        }

        private async void RestartBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = false;
                RestartButton.IsEnabled = false;
                // Restart the MQTT broker
                await _mqttServerService.StopBrokerAsync();
                await _mqttClientService.CloseConnectionAsync();
                PortconnecttionLabel.Content = "Không có kết nối";
                IPconnecttionSmallLabel.Content = $"Không có kết nối";
                IPconnecttionLabel.Content = "Không có kết nối";
                await Task.Delay(1000);
                await _mqttServerService.StartBrokerAsync();
                await _mqttClientService.ConnectAsync();
                ServerStatusLabel.Content = Constants.StatusOnline;
                PortconnecttionLabel.Content = "1883";
                string localIpAddress = GetLocalIpAddress();
                IPconnecttionSmallLabel.Content = $"Local IP: {localIpAddress}";
                IPconnecttionLabel.Content = $"{localIpAddress}";

                // Enable the Stop button
                RestartButton.IsEnabled = true;
                StopButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Show the error in a MessageBox
                MessageBox.Show($"{Constants.ErrorMessageBrokerRestart}: {ex}", Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                // Update the ServerStatusLabel to indicate an error
                ServerStatusLabel.Content = Constants.StatusError;
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.CurrentAccount = CurrentAccount;
            Hide();
            mainWindow.Show();
        }

        private void MqttService_ClientsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ConnectedClientsListBox.Items.Clear();

                IReadOnlyDictionary<string, string> connectedClients = _mqttServerService.GetConnectedClients();
                foreach (KeyValuePair<string, string> client in connectedClients)
                {
                    ConnectedClientsListBox.Items.Add($"{Constants.ClientIdLabel}: {client.Key}, {Constants.ClientIpLabel}: {client.Value}");
                }
            });
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
            MessageBox.Show("Bạn đang ở cửa sổ quản lý broker!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.CurrentAccount = CurrentAccount;
            Close();
            mainWindow.Show();
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
