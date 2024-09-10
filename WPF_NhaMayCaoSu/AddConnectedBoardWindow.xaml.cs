using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class AddConnectedBoardWindow : Window
    {
        private readonly MqttClientService _mqttClientService;
        private readonly MqttServerService _mqttServerService;
        private readonly BoardService _boardService;
        public Account CurrentAccount { get; set; } = null;
        private readonly Dictionary<string, string> _boardModes;

        public AddConnectedBoardWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            _boardService = new BoardService();
            _boardModes = new Dictionary<string, string>();

            // Replace the old event handler
            //_mqttServerService.ClientsChanged += OnClientsChanged;
            _mqttServerService.BoardReceived += OnClientsChanged;
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                SaveBoardButton.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("BoardInfo");

                // Subscribe to incoming MQTT messages
                _mqttClientService.MessageReceived += (s, data) =>
                {
                    Dispatcher.Invoke(() => ProcessMqttMessage(s, data));
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

        // New method to process incoming MQTT messages
        private async void ProcessMqttMessage(object sender, string data)
        {
            try
            {
                if (data.StartsWith("BoardInfo:"))
                {
                    string messageContent = data.Substring("BoardInfo:".Length);
                    Debug.WriteLine(messageContent);
                    string[] messages = messageContent.Split(':');

                    if (messages.Length == 2)
                    {
                        string macAddress = messages[0];
                        string currentMode = messages[1];

                        // Store the last received mode for the board
                        _boardModes[macAddress] = currentMode;

                        // Check if the board is already in the database
                        Board connectedBoard = await _boardService.GetBoardByMacAddressAsync(macAddress);
                        if (connectedBoard == null)
                        {
                            // Create a new board with the ClientID as the name
                            IReadOnlyDictionary<string, string> connectedClients = _mqttServerService.GetConnectedClients();
                            string clientName = connectedClients.FirstOrDefault(x => x.Value == macAddress).Key;

                            if (string.IsNullOrEmpty(clientName))
                            {
                                MessageBox.Show("Không tìm thấy Client ID cho địa chỉ MAC này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            connectedBoard = new Board
                            {
                                BoardId = Guid.NewGuid(),
                                BoardName = clientName,
                                BoardMacAddress = macAddress,
                                BoardIp = string.Empty // Assuming IP is not provided
                            };

                            Debug.WriteLine(connectedBoard);
                            await _boardService.CreateBoardAsync(connectedBoard);
                            MessageBox.Show("Board đã được thêm thành công.");
                        }
                        else
                        {
                            // Optionally, update board details if needed
                            await _boardService.UpdateBoardAsync(connectedBoard);
                            MessageBox.Show("Board đã được cập nhật thành công.");
                        }

                        // Reload the DataGrid to reflect the changes
                        LoadDataGrid();
                    }
                    else
                    {
                        Debug.WriteLine("Unexpected message format.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
            LoadDataGrid();
        }

        private void OnClientsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                List<BoardModelView> boards = _mqttServerService.GetConnectedBoard();
                boardDataGrid.ItemsSource = null;
                boardDataGrid.ItemsSource = boards;
            });
        }

        private async void SaveBoardButton_Click(object sender, RoutedEventArgs e)
        {
            if (boardDataGrid.SelectedItem is Board selectedBoard)
            {
                var existingBoard = await _boardService.GetBoardByNameAsync(selectedBoard.BoardName);
                if (existingBoard != null)
                {
                    MessageBox.Show("Board này đã được lưu", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    await _boardService.CreateBoardAsync(selectedBoard);
                    MessageBox.Show("Lưu board thành công", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một Board từ danh sách.", "No Board Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            LoadDataGrid();
        }

        private void LoadDataGrid()
        {

        }



    }
}
