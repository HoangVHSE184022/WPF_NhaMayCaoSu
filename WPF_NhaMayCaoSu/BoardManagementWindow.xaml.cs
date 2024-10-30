using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardManagementWindow.xaml
    /// </summary>
    public partial class BoardManagementWindow : Window
    {

        private IBoardService _service = new BoardService();

        public Account CurrentAccount { get; set; } = null;

        public Board SelectedBoard { get; set; } = null;


        public BoardManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn lưu Board này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            Board x = new Board
            {
                BoardName = BoardNameTextBox.Text,
                BoardIp = IpTextBox.Text,
                BoardMacAddress = MacAddressTextBox.Text,
                BoardMode = int.Parse(ModeTextBox.Text),
            };

            if (SelectedBoard == null)
            {
                await _service.CreateBoardAsync(x);
                MessageBox.Show("Tạo Board thành công", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                x.BoardId = SelectedBoard.BoardId;
                await _service.UpdateBoardAsync(x);
                MessageBox.Show("Chỉnh sửa Board thành công", Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);

            }

            Close();
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm Board mới";

            if (SelectedBoard != null)
            {
                BoardNameTextBox.Text = SelectedBoard.BoardName.ToString();
                IpTextBox.Text = SelectedBoard.BoardIp.ToString();
                IpTextBox.IsEnabled = false;
                MacAddressTextBox.Text = SelectedBoard.BoardMacAddress.ToString();
                MacAddressTextBox.IsEnabled = false;
                ModeTextBox.Text = SelectedBoard.BoardMode.ToString();
                ModeTextBox.IsEnabled = false;
                ModeLabel.Content = "Chỉnh sửa Board";
            }
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.CurrentAccount = CurrentAccount;
            Close();
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            Close();
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.ShowDialog();
            Close();
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
    }
}
