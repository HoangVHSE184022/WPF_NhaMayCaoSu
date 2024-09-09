<<<<<<< Updated upstream
﻿
using System.Diagnostics;
using WPF_NhaMayCaoSu.Core.Utils;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

=======
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
>>>>>>> Stashed changes

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class BoardListWindow : Window
    {
<<<<<<< Updated upstream
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private List<Board> _sessionBoardList { get; set; } = new();
=======
        private readonly MqttClientService _mqttClientService;
        private readonly MqttServerService _mqttServerService;
>>>>>>> Stashed changes
        public BoardListWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
<<<<<<< Updated upstream
            _mqttServerService.ClientsChanged += MqttService_ClientsChanged;
            _mqttClientService = new MqttClientService();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("BoardInfo");
                _mqttClientService.MessageReceived += OnMqttMessageReceived;

                _mqttClientService.MessageReceived += (s, data) =>
                {
                    OnMqttMessageReceived(s, data);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
            }
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private void MqttService_ClientsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                boardDataGrid.ItemsSource = null;

                IReadOnlyDictionary<string, string> connectedClients = _mqttServerService.GetConnectedClients();
                foreach (KeyValuePair<string, string> client in connectedClients)
                {
                    boardDataGrid.Items.Add($"{Constants.ClientIdLabel}: {client.Key}, {Constants.ClientIpLabel}: {client.Value}");
                }
            });
        }

        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                string messageContent = data.Substring("BoardInfo:".Length);
                string[] messages = messageContent.Split(':');

                if (messages.Length == 2)
                {
                    string MACAddress = messages[0];
                    int currentMode = int.Parse(messages[1]);
                    Board connectedBoard = new();

                    if (data.StartsWith("BoardInfo:"))
                    {
                        IReadOnlyDictionary<string, string> connectedClients = _mqttServerService.GetConnectedClients();

                        if (!string.IsNullOrEmpty(messageContent))
                        {
                            boardDataGrid.Dispatcher.Invoke(() =>
                            {
                                foreach (KeyValuePair<string, string> client in connectedClients)
                                {
                                    connectedBoard = new Board
                                    {
                                        BoardId = Guid.NewGuid(),
                                        BoardName = client.Key,
                                        BoardIp = client.Value,
                                        BoardMacAddress = MACAddress,
                                        BoardMode = currentMode
                                    };
                                    Debug.Write(connectedBoard);
                                    _sessionBoardList.Add(connectedBoard);
                                }
                                boardDataGrid.ItemsSource = null;
                                boardDataGrid.ItemsSource = _sessionBoardList;
                            });
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Failed to parse BoardInfo data.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }
=======
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("BoardInfo");


                _mqttClientService.MessageReceived += (s, data) =>
                {
                    OnMqttMessageReceived(s, data);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
                return;
            }
        }

        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("BoardInfo:"))
                {
                    string messageContent = data.Substring("BoardInfo:".Length);
                    string[] messages = messageContent.Split(':');

                    if (messages.Length == 2)
                    {
                        string MacAddress = messages[0];
                        int currentMode = int.Parse(messages[1]);
                        Board connectedBoard = null;
                    }
                    else
                    {
                        Debug.WriteLine("Unexpected message topic.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }

>>>>>>> Stashed changes
    }
}
