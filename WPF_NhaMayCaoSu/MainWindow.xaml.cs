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

        public MainWindow()
        {
            InitializeComponent();
            _mqttServerService = new MqttServerService();
            _mqttClientService = new MqttClientService();
            _mqttServerService.DeviceCountChanged += OnDeviceCountChanged;
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
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            DualCameraWindow dualCameraWindow = new DualCameraWindow(_cameraService);
            dualCameraWindow.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.CurrentAccount = CurrentAccount;
            window.Show();
        }

        private async void Broker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OpenServerButton.Content.ToString() == Constants.OpenServerText)
                {
                    // Start the MQTT broker
                    await _mqttServerService.StartBrokerAsync();

                    OpenServerButton.Content = Constants.CloseServerText;

                    await _mqttClientService.ConnectAsync();
                    ServerStatusTextBlock.Text = Constants.ServerOnlineStatus;
                }
                else
                {
                    // Stop the MQTT broker
                    await _mqttServerService.StopBrokerAsync();

                    OpenServerButton.Content = Constants.OpenServerText;

                    await _mqttClientService.CloseConnectionAsync();
                    ServerStatusTextBlock.Text = Constants.ServerOfflineStatus;
                }
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
                NumberofconnectionTextBlock.Text = $"Onl\n{deviceCount}";
            });
        }
    }
}





