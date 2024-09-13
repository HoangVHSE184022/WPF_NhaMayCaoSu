using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDListWindow.xaml
    /// </summary>
    public partial class RFIDListWindow : Window
    {
        public Account CurrentAccount { get; set; } = null;
        private IRFIDService _service = new RFIDService();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        public RFIDListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadDataGrid()
        {
            var sales = await _service.GetAllRFIDsAsync(_currentPage, _pageSize);
            int totalRFIDsCount = await _service.GetTotalRFIDsCountAsync();
            _totalPages = (int)Math.Ceiling((double)totalRFIDsCount / _pageSize);

            RFIDDataGrid.ItemsSource = null;
            RFIDDataGrid.Items.Clear();
            RFIDDataGrid.ItemsSource = await _service.GetAllRFIDsAsync(1, 10);

            PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";

            // Disable/Enable pagination buttons
            PreviousPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadDataGrid();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadDataGrid();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                AddRFIDButton.Visibility = Visibility.Collapsed;
                EditRFIDButton.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
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
            MessageBox.Show("Bạn đang ở cửa sổ Quản lý RFID!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.CurrentAccount = CurrentAccount;
            Close();
            mainWindow.Show();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.CurrentAccount = CurrentAccount;
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.SelectedRFID = RFIDDataGrid.SelectedItem as RFID;
            rFIDManagementWindow.CurrentAccount = CurrentAccount;
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadDataGrid();
            }
            else
            {
                RFIDDataGrid.ItemsSource = null;
                RFIDDataGrid.Items.Clear();
                var sales = await _service.GetAllRFIDsAsync(1, 10);
                RFIDDataGrid.ItemsSource = sales.Where(s => s.Customer.CustomerName.ToLower().Contains(searchTerm));
            }
        }

        private async void DeleteRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFID x = RFIDDataGrid.SelectedItem as RFID;

            if (x == null)
            {
                MessageBox.Show("Vui lòng chọn RFID để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await _service.DeleteRFIDAsync(x.RFID_Id);
            MessageBox.Show("Xoá RFID thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataGrid();

        }
    }
}
