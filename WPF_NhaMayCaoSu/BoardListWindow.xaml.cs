using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Services;
using Newtonsoft.Json;
using System.Net.Mail;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class BoardListWindow : Window
    {
        private readonly MqttClientService _mqttClientService;
        private readonly MqttServerService _mqttServerService;
        private readonly BoardService _boardService;
        public Account CurrentAccount { get; set; } = null;

        public BoardListWindow()
        {
            InitializeComponent();
            _mqttServerService = MqttServerService.Instance;
            _mqttClientService = new MqttClientService();
            _boardService = new BoardService();

            _mqttServerService.BoardReceived += MqttService_BoardsChanged;
        }

        private void MqttService_BoardsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                List<BoardModelView> boards = _mqttServerService.GetConnectedBoard();
                boardDataGrid.ItemsSource = null;
                boardDataGrid.ItemsSource = boards;
            });
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                SaveBoardButton.Visibility = Visibility.Collapsed;
                EditBoardButton.Visibility = Visibility.Collapsed;
            }
        }


        private void OnClientsChanged(object sender, EventArgs e)
        {
            /*            LoadDataGrid();*/
        }

        private async void SaveBoardButton_Click(object sender, RoutedEventArgs e)
        {
            AddConnectedBoardWindow addConnectedBoardWindow = new AddConnectedBoardWindow();
            addConnectedBoardWindow.CurrentAccount = CurrentAccount;
            addConnectedBoardWindow.ShowDialog();
            LoadDataGrid();
        }

        private async void EditBoardButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if a board is selected from the DataGrid
            if (boardDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một Board.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected board from the DataGrid
            BoardModelView? selectedBoard = boardDataGrid.SelectedItem as BoardModelView;
            Debug.WriteLine(selectedBoard.BoardMacAddress);
            Debug.WriteLine(string.IsNullOrEmpty(selectedBoard.BoardMacAddress));


            // Ensure selectedBoard is not null and has valid properties
            if (selectedBoard == null || string.IsNullOrEmpty(selectedBoard.BoardMacAddress))
            {
                MessageBox.Show("Board được chọn không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var board = new Board
                {
                    BoardId = selectedBoard.BoardId,
                    BoardName = selectedBoard.BoardName,
                    BoardIp = selectedBoard.BoardIp,
                    BoardMacAddress = selectedBoard.BoardMacAddress,
                    BoardMode = selectedBoard.BoardMode
                };
                // Check if the board exists in the database
                var existingBoard = await _boardService.GetBoardByMacAddressAsync(board.BoardMacAddress);
                if (existingBoard != null)
                {
                    MessageBox.Show("Board này đã được lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Save the board to the database
                await _boardService.CreateBoardAsync(board);
                MessageBox.Show("Lưu board thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu Board: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadDataGrid()
        {
            IEnumerable<Board> boards = await _boardService.GetAllBoardsAsync(1, 10);
            Application.Current.Dispatcher.Invoke(() =>
            {
                boardDataGrid.ItemsSource = null;
                boardDataGrid.ItemsSource = boards;
            });
        }

        private async void ModeBoardButton_Click(object sender, RoutedEventArgs e)
        {
            Board selected = boardDataGrid.SelectedItem as Board;

            var existingBoard = await _boardService.GetBoardByMacAddressAsync(selected.BoardMacAddress);
            if (existingBoard == null)
            {
                MessageBox.Show("Board này chưa được lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selected != null)
            {
                string topic = string.Empty;

                // Determine the topic based on the selected board name
                if (selected.BoardName == "ESP32_Can_ta")
                {
                    topic = "Canta_Mode";
                }
                else if (selected.BoardName == "ESP32_Can_tieu_ly")
                {
                    topic = "Cantieuly_Mode";
                }
                else
                {
                    MessageBox.Show("Unknown board selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selected.BoardMode = selected.BoardMode == 1 ? 2 : 1;

                var payloadObject = new { Mode = selected.BoardMode };
                string payload = JsonConvert.SerializeObject(payloadObject);

                if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                {
                    await _mqttClientService.PublishAsync(topic, payload);
                }

                boardDataGrid.Items.Refresh(); 
            }
            else
            {
                MessageBox.Show("Please select a board to publish.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        
    }
}
