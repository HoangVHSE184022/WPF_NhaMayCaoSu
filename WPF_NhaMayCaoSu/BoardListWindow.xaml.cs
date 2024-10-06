using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

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
            LoggingHelper.ConfigureLogger();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            _boardService = new BoardService();

            // Subscribe to board changes from MQTT
            _mqttServerService.BoardReceived += MqttService_BoardReceived;
            _mqttServerService.ClientsChanged += MqttService_BoardsChanged;

            try
            {
                LoadAwait();
                _mqttClientService.MessageReceived += (s, data) =>
                {
                    OnMqttMessageReceived(s, data);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, $"Không thể kết nối đến máy chủ MQTT");
                return;
            }

        }

        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);

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
                if (data.StartsWith("checkmode-"))
                {
                    string[] parts = data.Split('-');
                    string macAddress = parts[1];
                    int currentMode = int.Parse(parts[2]);

                    Board board = await _boardService.GetBoardByMacAddressAsync(macAddress);
                    if (board == null)
                    {
                        MessageBox.Show("Board này chưa được lưu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (board != null)
                    {
                        if (currentMode != board.BoardMode)
                        {
                            board.BoardMode = currentMode;
                            await _boardService.UpdateBoardAsync(board);
                            LoadDataGrid();
                            MessageBox.Show("Mode đã được chuyển thành công", "Thành công");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Lỗi khi sử dụng mã RFID.");
                    }
                }
                else
                {
                    Debug.WriteLine("Sai kiểu dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
                Log.Error(ex, $"Error processing message");
            }
        }

        private void MqttService_BoardsChanged(object sender, EventArgs e)
        {
            // Reload the DataGrid with updated boards
            Dispatcher.Invoke(() => { LoadDataGrid(); });
        }

        private async void LoadDataGrid()
        {
            IEnumerable<Board> boards = await _boardService.GetAllBoardsAsync(1, 10);
            boardDataGrid.ItemsSource = null;
            boardDataGrid.ItemsSource = boards;
        }

        private void MqttService_BoardReceived(object sender, EventArgs e)
        {
            Dispatcher.Invoke(async () =>
            {
                var mqttBoards = _mqttServerService.GetConnectedBoard();
                _mqttBoards = mqttBoards.ToList();  // Store received boards
                IEnumerable<Board> connectedBoardList = await _boardService.GetAllBoardsAsync(1, 10);

                _mqttBoards.RemoveAll(mqttBoard => connectedBoardList.Any(connectedBoard =>
                               connectedBoard.BoardMacAddress == mqttBoard.BoardMacAddress));

                ConnectedBoardDataGrid.ItemsSource = null;
                ConnectedBoardDataGrid.ItemsSource = _mqttBoards;
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (CurrentAccount?.Role?.RoleName != "Admin")
            //{
            //    EditBoardButton.Visibility = Visibility.Collapsed;
            //}
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

        private async void UnlockBoardButton_Click(object sender, RoutedEventArgs e)
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

            string topic = $"{selectedBoard.BoardMacAddress}/Save";
            var payloadObject = new { Save = 1 };
            string payload = JsonConvert.SerializeObject(payloadObject);
            if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
            {
                try
                {
                    await _mqttClientService.PublishAsync(topic, payload);
                }
                catch (Exception publishEx)
                {
                    Debug.WriteLine($"Error publishing message in catch block: {publishEx.Message}");
                    Log.Error(publishEx, "Error publishing message in catch block");
                }
            }
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
            await _boardService.UpdateBoardAsync(selectedBoard);

            string topic = $"{selectedBoard.BoardMacAddress}/mode";


            var payloadObject = new { Mode = selectedBoard.BoardMode };
            string payload = JsonConvert.SerializeObject(payloadObject);

            if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
            {
                await _mqttClientService.PublishAsync(topic, payload);
                await _boardService.UpdateBoardAsync(selectedBoard);
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
                    Log.Error(ex, $"Lỗi khi xóa Board");
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
                // Check if the board already exists in the system
                Board existingBoard = await _boardService.GetBoardByMacAddressAsync(selectedBoard.BoardMacAddress);
                if (existingBoard == null)
                {
                    // Open the SaveBoardWindow to ask the user for the board name
                    SaveBoardWindow saveBoardWindow = new SaveBoardWindow();
                    bool? result = saveBoardWindow.ShowDialog();

                    if (result == true && !string.IsNullOrEmpty(saveBoardWindow.SelectedBoardName))
                    {

                        //Check dublicate boardname
                        Board board = await _boardService.GetBoardByNameAsync(saveBoardWindow.SelectedBoardName);
                        if (board != null)
                        {
                            MessageBoxResult re = MessageBox.Show("Bạn có muốn thay thế cho board cũ không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (re == MessageBoxResult.Yes)
                            {
                                board.BoardMacAddress = selectedBoard.BoardMacAddress;
                                await _boardService.UpdateBoardAsync(board);
                                await LoadDataGridFromDatabase();

                            }
                            return;
                        }


                        // Create a new board and save the selected name
                        Board newBoard = new Board
                        {
                            BoardId = selectedBoard.BoardId,
                            BoardName = saveBoardWindow.SelectedBoardName, // Use the name selected by the user
                            BoardIp = selectedBoard.BoardIp,
                            BoardMacAddress = selectedBoard.BoardMacAddress,
                            BoardMode = selectedBoard.BoardMode
                        };

                        await _boardService.CreateBoardAsync(newBoard);
                        MessageBox.Show("Board đã được thêm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Reload the data grid
                        await LoadDataGridFromDatabase();
                        _mqttBoards.Clear();
                        ConnectedBoardDataGrid.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một tên cho board.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Board này đã tồn tại trong hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private async void ControlButton_Click(object sender, RoutedEventArgs e)
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

        private async void SwitchButton_Click(object sender, RoutedEventArgs e)
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
                await _boardService.UpdateBoardAsync(selectedBoard);
            }




            boardDataGrid.Items.Refresh();
        }

    }
}