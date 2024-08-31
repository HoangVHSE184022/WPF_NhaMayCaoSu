﻿using System.Windows;
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
            _mqttServerService = new MqttServerService();
            _mqttServerService.ClientsChanged += MqttService_ClientsChanged;
            _mqttClientService = new MqttClientService();
        }

        private async void StartBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Start the MQTT broker
                await _mqttServerService.StartBrokerAsync();

                // Update the ServerStatusLabel to "Online"
                ServerStatusLabel.Content = Constants.StatusOnline;

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
                // Restart the MQTT broker
                await _mqttServerService.RestartBrokerAsync();
                ServerStatusLabel.Content = Constants.StatusOnline;

                await _mqttClientService.ConnectAsync();

                // Disable the Start button when the server is restarted
                StartButton.IsEnabled = false;

                // Enable the Stop button
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
            Close();
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
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            rFIDListWindow.ShowDialog();
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

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.CurrentAccount = CurrentAccount;
            mainWindow.Show();
        }
    }
}
