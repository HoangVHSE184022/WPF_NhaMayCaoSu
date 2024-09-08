
using System.Diagnostics;
using WPF_NhaMayCaoSu.Core.Utils;
using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;


namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class BoardListWindow : Window
    {
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private List<Board> _sessionBoardList { get; set; } = new();
        public BoardListWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
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
    }
}
