using System;
using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu.Content
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : Window
    {
        public Account CurrentAccount { get; set; }

        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private readonly BrokerWindow broker;
        private CustomerListWindow customerListWindow;
        private SaleListWindow saleListWindow;
        private AccountManagementWindow accountManagementWindow;
        private RFIDListWindow rfidListWindow;
        private RoleListWindow roleListWindow;
        private MainWindow mainWindow;
        private ConfigCamera configCamera;

        public MainControl()
        {
            InitializeComponent();

            broker = new BrokerWindow();
            customerListWindow = new CustomerListWindow();
            saleListWindow = new SaleListWindow();
            accountManagementWindow = new AccountManagementWindow();
            rfidListWindow = new RFIDListWindow();
            roleListWindow = new RoleListWindow();
            mainWindow = new();
            configCamera = new();

            mainWindow.CurrentAccount = CurrentAccount;
            broker.CurrentAccount = CurrentAccount;
            customerListWindow.CurrentAccount = CurrentAccount;
            saleListWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.CurrentAccount = CurrentAccount;
            rfidListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.CurrentAccount = CurrentAccount;

            MainContentControl.Content = broker.Content;
            _mqttServerService = MqttServerService.Instance;
            _mqttServerService.BrokerStatusChanged += (sender, e) => UpdateMainWindowUI();
            _mqttClientService = new MqttClientService();
            _mqttServerService.DeviceCountChanged += OnDeviceCountChanged;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool ValidCheck()
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        // Toggle the visibility of BrokerWindow content
        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            
            MainContentControl.Content = broker.Content;
            broker.OnWindowLoaded();
            this.Title = broker.Title;
            
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = customerListWindow.Content;
            customerListWindow.OnWindowLoaded();
            this.Title = customerListWindow.Title;
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = saleListWindow.Content;
            saleListWindow.OnWindowLoaded();
            this.Title = saleListWindow.Title;
        }

        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = accountManagementWindow.Content;
            accountManagementWindow.OnWindowLoaded();
            this.Title = accountManagementWindow.Title;
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = rfidListWindow.Content;
            rfidListWindow.OnWindowLoaded();
            this.Title = rfidListWindow.Title;
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = roleListWindow.Content;
            roleListWindow.OnWindowLoaded();
            this.Title = roleListWindow.Title;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = configCamera.Content;
            configCamera.OnWindowLoaded();
            this.Title = configCamera.Title;
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidCheck())
            {
                return;
            }

            MainContentControl.Content = mainWindow.Content;
            mainWindow.OnWindowLoaded();
            this.Title = mainWindow.Title;
        }

        //private void LoginButton_Click(object sender, RoutedEventArgs e)
        //{
        //    LoginWindow window = new();
        //    window.LoginSucceeded += HandleLoginSucceeded;
        //    window.ShowDialog();
        //}
        //private void HandleLoginSucceeded(Account account)
        //{
        //    CurrentAccount = account;
        //    mainWindow.CurrentAccount = account;
        //    broker.CurrentAccount = account;
        //    customerListWindow.CurrentAccount = account;
        //    saleListWindow.CurrentAccount = account;
        //    accountManagementWindow.CurrentAccount = account;
        //    rfidListWindow.CurrentAccount = account;
        //    roleListWindow.CurrentAccount = account;
        //    //LoginButton.Visibility = Visibility.Hidden;

        //    MessageBox.Show("Đăng nhập tài khoản thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //}

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

        private void OnDeviceCountChanged(object sender, int deviceCount)
        {
            Dispatcher.Invoke(() =>
            {
                NumberofconnectionTextBlock.Text = $"Onl:{deviceCount} Thiết bị";
            });
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
    }

}
