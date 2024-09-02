using System.Windows;
using System.Windows.Controls;
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
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private readonly CameraService _cameraService = new();
        private readonly List<Sale> _sessionSaleList;
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
            _sessionSaleList = new();
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SalesDataGrid.ItemsSource = null;
                    SalesDataGrid.ItemsSource = _mqttClientService._sessionSaleList;
                });
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
        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
            }
            int choice = 0;
            Sale scannedSale = new();
            if (SalesDataGrid.SelectedItem is Sale selectedSale)
            {
                scannedSale = selectedSale;
                if(selectedSale.ProductWeight == null && selectedSale.ProductDensity == null)
                {
                    choice = 1;
                }
                else if(selectedSale.ProductDensity == null && selectedSale.ProductWeight != null)
                {
                    choice = 2;
                }
                else if(selectedSale.ProductWeight == null && selectedSale.ProductDensity != null)
                {
                     choice = 3;
                }
            }
            switch(choice){
                case 1:
                    CreateRFID(scannedSale);
                    break;
                case 2:
                    AddWeight(scannedSale);
                    break;
                case 3:
                    AddDensity(scannedSale);
                    break;
                default:
                    break;
            }
        }
        private void CreateRFID(Sale sale)
        {
            RFIDManagementWindow window = new();
            window.ShowDialog();
        }
        private async void AddWeight(Sale sale)
        {
            //Tạo 2 trường hợp if else cho 2 trường hợp: Tạo và cập nhật


            MessageBox.Show("Dữ liệu cân tạ cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }
        private async void AddDensity(Sale sale)
        {
            //function to automagically add Density to db
            //await.......

            MessageBox.Show("Dữ liệu cân tiểu li cập nhật thành công","Cập nhật dữ liệu",MessageBoxButton.OK);
        }
    }
}





