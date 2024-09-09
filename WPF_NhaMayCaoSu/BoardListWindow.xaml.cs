using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
using WPF_NhaMayCaoSu.Service.Interfaces;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class BoardListWindow : Window
    {
        private readonly MqttClientService _mqttClientService;
        private readonly MqttServerService _mqttServerService;
        private readonly IBoardService _boardService;
        public BoardListWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            _boardService = new BoardService();
            _mqttServerService.ClientsChanged += OnMqttMessageReceived;
        }
        public Account CurrentAccount { get; set; } = null;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                AddBoardButton.Visibility = Visibility.Collapsed;
                EditBoardButton.Visibility = Visibility.Collapsed;
            }
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
                        connectedBoard = await
                            if (connectedBoard == null)
                        {
                            connectedBoard = new Board
                            {

                            };
                            await _boardService.CreateBoardAsync(connectedBoard);
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

    }
}


