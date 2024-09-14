using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Services;
using Newtonsoft.Json;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    public partial class BoardListWindow : Window
    {
        private readonly MqttClientService _mqttClientService;
        private readonly MqttServerService _mqttServerService;
        private readonly IBoardService _boardService;
        public Account CurrentAccount { get; set; } = null;

        // List to store boards received from MQTT
        private List<BoardModelView> _mqttBoards = new();

        public BoardListWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            _boardService = new BoardService();

            // Subscribe to board changes from MQTT
            _mqttServerService.BoardReceived += MqttService_BoardReceived;
            _mqttServerService.ClientsChanged += MqttService_BoardsChanged;

            try
            {
               // LoadAwait();
                _mqttClientService.MessageReceived += (s, data) =>
                {
                    OnMqttMessageReceived(s, data);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

        }
        private async void LoadAwait()
        {
            string topic = "+/checkmode";
            await _mqttClientService.ConnectAsync();
            await _mqttClientService.SubscribeAsync(topic);
        }

        private async void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                var messageParts = data.Split(',');
                string macAddress = messageParts.FirstOrDefault(x => x.StartsWith("MacAddress"))?.Split(':')[1];
                int mode = int.Parse(messageParts.FirstOrDefault(x => x.StartsWith("Mode"))?.Split(':')[1]);

                if (!string.IsNullOrEmpty(macAddress))
                {
                    Board board = await _boardService.GetBoardByMacAddressAsync(macAddress);
                    Debug.WriteLine($"MacAddress: {macAddress}, Mode: {mode}");
                    if (board != null)
                    {
                        if(board.BoardMode != mode)
                        {
                            board.BoardMode = mode;
                            await _boardService.UpdateBoardAsync(board);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        private void MqttService_BoardsChanged(object sender, EventArgs e)
        {
            // Reload the DataGrid with updated boards
            Dispatcher.Invoke(() => { LoadDataGrid(); });
        }

        private async void LoadDataGrid()
        {
            // Retrieve the boards from the database using BoardService
            IEnumerable<Board> boards = await _boardService.GetAllBoardsAsync(1, 10);

            Application.Current.Dispatcher.Invoke(() =>
            {
                boardDataGrid.ItemsSource = null; // Clear the DataGrid
                boardDataGrid.ItemsSource = boards; // Bind the DataGrid to the retrieved boards
            });
        }

        // Handle MQTT boards received and show them in the right DataGrid
        private void MqttService_BoardReceived(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var mqttBoards = _mqttServerService.GetConnectedBoard();
                _mqttBoards = mqttBoards.ToList();  // Store received boards

                // Load MQTT-received boards into the right DataGrid
                ConnectedBoardDataGrid.ItemsSource = null;
                ConnectedBoardDataGrid.ItemsSource = _mqttBoards;
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                EditBoardButton.Visibility = Visibility.Collapsed;
            }
            // Load boards from database into the left DataGrid
            await LoadDataGridFromDatabase();
        }

        private async Task LoadDataGridFromDatabase()
        {
            // Retrieve the boards from the database using BoardService
            IEnumerable<Board> boards = await _boardService.GetAllBoardsAsync(1, 10);
            Application.Current.Dispatcher.Invoke(() =>
            {
                boardDataGrid.ItemsSource = null; // Clear the DataGrid
                boardDataGrid.ItemsSource = boards; // Bind the DataGrid to the retrieved boards
            });
        }

        private void EditBoardButton_Click(object sender, RoutedEventArgs e)
        {
            BoardManagementWindow boardManagementWindow = new BoardManagementWindow();
            boardManagementWindow.CurrentAccount = CurrentAccount;
            boardManagementWindow.SelectedBoard = boardDataGrid.SelectedItem as Board;
            boardManagementWindow.ShowDialog();
            LoadDataGridFromDatabase();
        }

        private async void ModeBoardButton_Click(object sender, RoutedEventArgs e)
        {
            if (boardDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một Board.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected board from the left DataGrid (from database)
            Board selectedBoard = boardDataGrid.SelectedItem as Board;

            if (selectedBoard == null)
            {
                MessageBox.Show("Board được chọn không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingBoard = await _boardService.GetBoardByMacAddressAsync(selectedBoard.BoardMacAddress);
            if (existingBoard == null)
            {
                MessageBox.Show("Board này chưa được lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Toggle the board mode and update it
            selectedBoard.BoardMode = selectedBoard.BoardMode == 1 ? 2 : 1;

            string topic = $"/{selectedBoard.BoardMacAddress}/mode";


            var payloadObject = new { Mode = selectedBoard.BoardMode };
            string payload = JsonConvert.SerializeObject(payloadObject);

            if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
            {
                await _mqttClientService.PublishAsync(topic, payload);
            }

            boardDataGrid.Items.Refresh(); 
        }

        private async void DeleteBoardButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected board from the left DataGrid
            if (boardDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một Board.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Board selectedBoard = boardDataGrid.SelectedItem as Board;
            if (selectedBoard == null || string.IsNullOrEmpty(selectedBoard.BoardMacAddress))
            {
                MessageBox.Show("Board được chọn không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa board này không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _boardService.DeleteBoardAsync(selectedBoard.BoardId);
                    MessageBox.Show("Board đã được xóa thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload DataGrid after deletion
                    await LoadDataGridFromDatabase();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa Board: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Add selected board from the right DataGrid (MQTT-received) to the database
        private async void AddBoardButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectedBoardDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một Board từ danh sách Boards kết nối.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BoardModelView selectedBoard = ConnectedBoardDataGrid.SelectedItem as BoardModelView;

            if (selectedBoard != null)
            {
                var existingBoard = await _boardService.GetBoardByMacAddressAsync(selectedBoard.BoardMacAddress);
                if (existingBoard == null)
                {
                    var newBoard = new Board
                    {
                        BoardId = selectedBoard.BoardId,
                        BoardName = selectedBoard.BoardName,
                        BoardIp = selectedBoard.BoardIp,
                        BoardMacAddress = selectedBoard.BoardMacAddress,
                        BoardMode = selectedBoard.BoardMode
                    };

                    // Save the selected board to the database
                    await _boardService.CreateBoardAsync(newBoard);
                    MessageBox.Show("Board đã được thêm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload left DataGrid after adding new board
                    await LoadDataGridFromDatabase();
                }
                else
                {
                    MessageBox.Show("Board này đã tồn tại trong hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}