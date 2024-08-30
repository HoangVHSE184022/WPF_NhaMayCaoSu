using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BrokerWindow.xaml
    /// </summary>
    public partial class BrokerWindow : Window
    {
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
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
                Console.WriteLine("Broker started");

                // Update the ServerStatusLabel to "Online"
                ServerStatusLabel.Content = "Online";

                await _mqttClientService.ConnectAsync();

                // Disable the Start button when the server is online
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Log the error to the console
                Console.WriteLine($"Failed to start broker: {ex.Message}");

                // Show the error in a MessageBox
                MessageBox.Show($"An error occurred while starting the broker: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.ToString());

                // Update the ServerStatusLabel to indicate an error
                ServerStatusLabel.Content = "Error";
            }
        }

        private async void StopBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop the MQTT broker
                await _mqttServerService.StopBrokerAsync();
                Console.WriteLine("Broker stopped");

                // Update the ServerStatusLabel to "Offline"
                ServerStatusLabel.Content = "Offline";

                await _mqttClientService.CloseConnectionAsync();

                // Enable the Start button when the server is offline
                StartButton.IsEnabled = true;

                StopButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop broker: {ex.Message}");
                MessageBox.Show($"An error occurred while starting the broker: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.ToString());

                ServerStatusLabel.Content = "Error";
            }
        }

        private async void RestartBroker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Restart the MQTT broker
                await _mqttServerService.RestartBrokerAsync();
                ServerStatusLabel.Content = "Online";

                await _mqttClientService.ConnectAsync();

                // Disable the Start button when the server is restarted
                StartButton.IsEnabled = false;

                // Enable the Stop button
                StopButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                // Log the error to the console
                Console.WriteLine($"Failed to stop broker: {ex.Message}");

                // Show the error in a MessageBox
                MessageBox.Show($"An error occurred while starting the broker: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.ToString());

                // Update the ServerStatusLabel to indicate an error
                ServerStatusLabel.Content = "Error";
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MqttService_ClientsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ConnectedClientsListBox.Items.Clear();

                IReadOnlyDictionary<string, string> connectedClients = _mqttServerService.GetConnectedClients();
                foreach (KeyValuePair<string, string> client in connectedClients)
                {
                    ConnectedClientsListBox.Items.Add($"Client ID: {client.Key}, IP: {client.Value}");
                }
            });
        }
    }
}
