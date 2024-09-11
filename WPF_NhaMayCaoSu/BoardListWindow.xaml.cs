using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Services;
using Newtonsoft.Json;

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

            // Subscribe to board changes
            _mqttServerService.ClientsChanged += MqttService_BoardsChanged;
        }

        private void MqttService_BoardsChanged(object sender, EventArgs e)
        {
            // Reload the DataGrid with updated boards
            Dispatcher.Invoke(() => { LoadDataGrid(); });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                SaveBoardButton.Visibility = Visibility.Collapsed;
                EditBoardButton.Visibility = Visibility.Collapsed;
            }
            // Load data from database when window loads
            LoadDataGrid();
        }

        private async void SaveBoardButton_Click(object sender, RoutedEventArgs e)
        {
            AddConnectedBoardWindow addConnectedBoardWindow = new AddConnectedBoardWindow();
            addConnectedBoardWindow.CurrentAccount = CurrentAccount;
            addConnectedBoardWindow.ShowDialog();
            LoadDataGrid(); // Reload the DataGrid after adding new boards
        }

        private void EditBoardButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng này đang trong quá trình phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        //if (boardDataGrid.SelectedItem == null)
        //{
        //    MessageBox.Show("Vui lòng chọn một Board.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    return;
        //}

        //// Get the selected board from the DataGrid
        //Board selectedBoard = boardDataGrid.SelectedItem as Board;

        //// Ensure the board is valid and exists
        //if (selectedBoard == null || string.IsNullOrEmpty(selectedBoard.BoardMacAddress))
        //{
        //    MessageBox.Show("Board được chọn không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //    return;
        //}

        //try
        //{
        //    var existingBoard = await _boardService.GetBoardByMacAddressAsync(selectedBoard.BoardMacAddress);
        //    if (existingBoard != null)
        //    {
        //        MessageBox.Show("Board này đã được lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    // Save the new board
        //    await _boardService.CreateBoardAsync(selectedBoard);
        //    MessageBox.Show("Lưu board thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Lỗi khi lưu Board: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        //}
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

        private async void ModeBoardButton_Click(object sender, RoutedEventArgs e)
        {
            if (boardDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một Board.", "Không có Board được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected board from the DataGrid
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

            string topic = string.Empty;

            if (selectedBoard.BoardName == "ESP32_Canta")
            {
                topic = "Canta_Mode";
            }
            else if (selectedBoard.BoardName == "ESP32_Cantieuly")
            {
                topic = "Cantieuly_Mode";
            }
            else
            {
                MessageBox.Show("Unknown board selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var payloadObject = new { Mode = selectedBoard.BoardMode };
            string payload = JsonConvert.SerializeObject(payloadObject);

            // Publish the new mode to the MQTT topic
            if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
            {
                await _mqttClientService.PublishAsync(topic, payload);
            }

            boardDataGrid.Items.Refresh(); // Refresh the DataGrid to show updated mode
        }
    }
}
