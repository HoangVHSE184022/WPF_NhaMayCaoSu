using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Serilog;
using System.Diagnostics;
using System.IO;
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
        private IBoardService _boardService = new BoardService();
        private readonly MqttClientService _mqttClientService;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages;
        public RFIDListWindow()
        {
            InitializeComponent();
            _mqttClientService = new();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadDataGrid()
        {
            try
            {
                IEnumerable<RFID> paginatedRFIDs = await _service.GetAllRFIDsAsync(_currentPage, _pageSize);

                List<RFID> rfid = paginatedRFIDs.ToList();

                int totalRFIDsCount = rfid.Count;
                _totalPages = (int)Math.Ceiling((double)totalRFIDsCount / _pageSize);

                RFIDDataGrid.ItemsSource = null;
                RFIDDataGrid.Items.Clear();
                RFIDDataGrid.ItemsSource = rfid;
                PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
                PreviousPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages;
            }
            catch (Exception ex)
            {
                // Handle errors (consider logging)
                Debug.WriteLine($"Error loading RFIDs: {ex.Message}");
                Log.Error(ex, "Error loading RFIDs");
            }
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
            LoadCurrentBoardModeAsync();
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
            MessageBoxResult result = MessageBox.Show("Bạn có muốn xoá RFID này?", "Xoá RFID", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            await _service.DeleteRFIDAsync(x.RFID_Id);
            MessageBox.Show("Xoá RFID thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataGrid();

        }

        private async Task LoadCurrentBoardModeAsync()
        {
            // Assuming that Board has a Mode property and we are fetching the newest one
            Board newestBoard = await _boardService.GetBoardByNameAsync("Cân Tạ");
            Board newestBoard2 = await _boardService.GetBoardByNameAsync("Cân Tiểu Ly");
            if (newestBoard != null)
            {
                // Set the mode labels based on the newest board's current mode
                CantaMode.Content = $"Mode Cân \nTạ: {newestBoard.BoardMode}";
            }
            else
            {
                CantaMode.Content = "Chưa có \nCân Tạ";
            }

            if (newestBoard2 != null)
            {
                // Set the mode labels based on the newest board's current mode
                CantieulyMode.Content = $"Mode Cân \nTiểu Ly: {newestBoard2.BoardMode}";
            }
            else
            {
                CantieulyMode.Content = "Chưa có \nCân Tiểu Ly";
            }
        }

        // Change Cân Tạ mode
        private async void ChangeCanTaMode_Click(object sender, RoutedEventArgs e)
        {
            Board newestBoard = await _boardService.GetBoardByNameAsync("Cân Tạ");
            if (newestBoard != null)
            {
                newestBoard.BoardMode = newestBoard.BoardMode == 1 ? 2 : 1;

                string topic = $"{newestBoard.BoardMacAddress}/mode";


                var payloadObject = new { Mode = newestBoard.BoardMode };
                string payload = JsonConvert.SerializeObject(payloadObject);

                if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                {
                    await _mqttClientService.PublishAsync(topic, payload);
                    await _boardService.UpdateBoardAsync(newestBoard);
                }

                await LoadCurrentBoardModeAsync();
            }
        }

        // Change Cân Tiểu Ly mode
        private async void ChangeCanTieuLyMode_Click(object sender, RoutedEventArgs e)
        {
            Board newestBoard = await _boardService.GetBoardByNameAsync("Cân Tiểu Ly");
            if (newestBoard != null)
            {
                newestBoard.BoardMode = newestBoard.BoardMode == 1 ? 2 : 1;

                string topic = $"{newestBoard.BoardMacAddress}/mode";


                var payloadObject = new { Mode = newestBoard.BoardMode };
                string payload = JsonConvert.SerializeObject(payloadObject);

                if (!string.IsNullOrEmpty(topic) && !string.IsNullOrEmpty(payload))
                {
                    await _mqttClientService.PublishAsync(topic, payload);
                    await _boardService.UpdateBoardAsync(newestBoard);
                }

                await LoadCurrentBoardModeAsync();
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rfids = await _service.GetAllRFIDsAsync();
                var filteredRFIDs = rfids.Where(r => r.Status == 1).ToList();

                if (filteredRFIDs.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("RFID Data");

                    var header = new List<string> { "Số thứ tự", "RFID_Id", "Mã RFID", "Ngày tạo", "Ngày hết hạn", "Tên khách hàng" };

                    for (int i = 0; i < header.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = header[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i + 1].AutoFitColumns();
                    }


                    for (int i = 0; i < filteredRFIDs.Count; i++)
                    {
                        var rfid = filteredRFIDs[i];

                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = rfid.RFID_Id;
                        worksheet.Cells[i + 2, 3].Value = rfid.RFIDCode;
                        worksheet.Cells[i + 2, 4].Value = rfid.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cells[i + 2, 5].Value = rfid.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cells[i + 2, 6].Value = rfid.Customer?.CustomerName ?? "Không có khách hàng";
                    }

                    for (int col = 1; col <= header.Count; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CaoSuData");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, $"RFIDData_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    File.WriteAllBytes(filePath, package.GetAsByteArray());

                    MessageBox.Show($"Xuất file Excel thành công tại: {filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình xuất file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Error exporting RFIDs to Excel");
            }
        }

    }
}
