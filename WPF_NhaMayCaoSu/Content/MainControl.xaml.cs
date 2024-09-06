using System;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Content
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : Window
    {
        public Account CurrentAccount { get; set; }

        // Keep instances of windows (UserControls recommended) as class-level references
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

            // Initialize the windows (or UserControls)
            broker = new BrokerWindow();
            customerListWindow = new CustomerListWindow();
            saleListWindow = new SaleListWindow();
            accountManagementWindow = new AccountManagementWindow();
            rfidListWindow = new RFIDListWindow();
            roleListWindow = new RoleListWindow();
            mainWindow = new();
            configCamera = new();

            // Set CurrentAccount for each window once
            mainWindow.CurrentAccount = CurrentAccount;
            broker.CurrentAccount = CurrentAccount;
            customerListWindow.CurrentAccount = CurrentAccount;
            saleListWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.CurrentAccount = CurrentAccount;
            rfidListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.CurrentAccount = CurrentAccount;

            // Show broker content by default
            MainContentControl.Content = broker.Content;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // Toggle the visibility of BrokerWindow content
        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first","Please login",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if(MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = broker.Content;
            broker.OnWindowLoaded();
            this.Title = broker.Title;
            
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MainContentControl.Content = customerListWindow.Content;
            customerListWindow.OnWindowLoaded();
            this.Title = customerListWindow.Title;
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = saleListWindow.Content;
            saleListWindow.OnWindowLoaded();
            this.Title = saleListWindow.Title;
        }

        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = accountManagementWindow.Content;
            accountManagementWindow.OnWindowLoaded();
            this.Title = accountManagementWindow.Title;
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = rfidListWindow.Content;
            rfidListWindow.OnWindowLoaded();
            this.Title = rfidListWindow.Title;
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = roleListWindow.Content;
            roleListWindow.OnWindowLoaded();
            this.Title = roleListWindow.Title;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = configCamera.Content;
            configCamera.OnWindowLoaded();
            this.Title = configCamera.Title;
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount is null)
            {
                MessageBox.Show("You must be logged in first", "Please login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (MqttServerService.IsBrokerRunning == false)
            {
                MessageBox.Show("You must start the server first", "Sever status offline", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainContentControl.Content = mainWindow.Content;
            mainWindow.OnWindowLoaded();
            this.Title = mainWindow.Title;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new();
            window.LoginSucceeded += HandleLoginSucceeded;
            window.ShowDialog();
        }
        private void HandleLoginSucceeded(Account account)
        {
            CurrentAccount = account;
            mainWindow.CurrentAccount = account;
            broker.CurrentAccount = account;
            customerListWindow.CurrentAccount = account;
            saleListWindow.CurrentAccount = account;
            accountManagementWindow.CurrentAccount = account;
            rfidListWindow.CurrentAccount = account;
            roleListWindow.CurrentAccount = account;
            LoginButton.Visibility = Visibility.Hidden;

            MessageBox.Show("Đăng nhập tài khoản thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
