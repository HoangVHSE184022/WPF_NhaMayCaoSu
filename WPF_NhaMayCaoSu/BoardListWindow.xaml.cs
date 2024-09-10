using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;
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
   /*         LoadDataGrid();*/
        }

        private async void EditBoardButton_Click(object sender, RoutedEventArgs e)
        {
            /*LoadDataGrid();*/
        }

        /*   private async void LoadDataGrid()
        {
            IEnumerable<Board> boards = await _boardService.GetAllBoardsAsync(1, 10);
            boardDataGrid.ItemsSource = null;
            boardDataGrid.ItemsSource = boards;
        }*/

    }
}
