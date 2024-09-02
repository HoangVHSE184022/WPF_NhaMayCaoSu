using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SessionService _sessionService;
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private readonly CameraService _cameraService = new();
        public Account CurrentAccount { get; set; } = null;
        private BrokerWindow broker;

        public MainWindow()
        {
            InitializeComponent();
            broker = new BrokerWindow();
            _mqttServerService = MqttServerService.Instance;
            _mqttServerService.BrokerStatusChanged += (sender, e) => UpdateMainWindowUI();
            _mqttClientService = new MqttClientService();
            _mqttServerService.DeviceCountChanged += OnDeviceCountChanged;
            UpdateMainWindowUI();
        }
        private void UpdateMainWindowUI()
        {
            if (MqttServerService.IsBrokerRunning)
            {
                OpenServerButton.Content = Constants.CloseServerText;
                ServerStatusTextBlock.Text = Constants.ServerOnlineStatus;
                int deviceCount = _mqttServerService.GetDeviceCount();
                NumberofconnectionTextBlock.Text = $"Onl: {deviceCount} Thiết bị";
            }
            else
            {
                OpenServerButton.Content = Constants.OpenServerText;
                ServerStatusTextBlock.Text = Constants.ServerOfflineStatus;
            }
        }

        public void FoundEvent(Sale sale)
        {
            _sessionService.AddToSalelist(sale);
            SalesDataGrid.ItemsSource = null;
            SalesDataGrid.ItemsSource = _sessionService.GetAllSales();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            customerListWindow.ShowDialog();
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

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            rFIDListWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            broker.Show();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private async void Broker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MqttServerService.IsBrokerRunning)
                {
                    await _mqttServerService.StopBrokerAsync();
                    await _mqttClientService.CloseConnectionAsync();
                }
                else
                {
                    await _mqttServerService.StartBrokerAsync();
                    await _mqttClientService.ConnectAsync();
                }
                UpdateMainWindowUI();
            }
            catch (Exception ex)
            {
                if (OpenServerButton.Content.ToString() == Constants.OpenServerText)
                {
                    MessageBox.Show(Constants.BrokerStartErrorMessage + "\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(Constants.BrokerStopErrorMessage + "\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnDeviceCountChanged(object sender, int deviceCount)
        {
            Dispatcher.Invoke(() =>
            {
                NumberofconnectionTextBlock.Text = $"Onl:{deviceCount} Thiết bị";
            });
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
    }
}





