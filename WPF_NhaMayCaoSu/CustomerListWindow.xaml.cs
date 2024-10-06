using OfficeOpenXml;
using OfficeOpenXml.Style;
using Serilog;
using System.IO;
using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {

        private ICustomerService _service = new CustomerService();
        private int _currentPage = 1;
        private int _pageSize = 12;
        private int _totalPages;
        public Account CurrentAccount { get; set; } = null;

        public CustomerListWindow()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.CurrentAccount = CurrentAccount;
            customerManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditCustomerButton1_Click(object sender, RoutedEventArgs e)
        {

            Customer selected = CustomerDataGrid.SelectedItem as Customer;

            if (selected == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectCustomer, Constants.ErrorTitleSelectCustomer, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.CurrentAccount = CurrentAccount;
            customerManagementWindow.SelectedCustomer = selected;
            customerManagementWindow.ShowDialog();
            LoadDataGrid();
        }


        private async void LoadDataGrid()
        {
            int totalCustomerCount = await _service.GetTotalCustomersCountAsync();
            _totalPages = (int)Math.Ceiling((double)totalCustomerCount / _pageSize);

            CustomerDataGrid.ItemsSource = null;
            CustomerDataGrid.Items.Clear();
            CustomerDataGrid.ItemsSource = await _service.GetAllCustomers(_currentPage, _pageSize);

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
                EditCustomerButton1.Visibility = Visibility.Collapsed;
                AddCustomerButton.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đang ở cửa sổ Quản lý Khách hàng!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
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
            mainWindow.CurrentAccount = CurrentAccount;
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName == "User")
            {
                MessageBox.Show(Constants.UnauthorizedMessage, Constants.UnauthorizedTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Customer selectedCustomer = CustomerDataGrid.SelectedItem as Customer;

            if (selectedCustomer != null)
            {
                RFIDManagementWindow rfidManagementWindow = new RFIDManagementWindow(selectedCustomer);
                rfidManagementWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Xin hãy chọn khách hàng trước.", "Lưu ý", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoadDataGrid();
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
                CustomerDataGrid.ItemsSource = null;
                CustomerDataGrid.Items.Clear();
                var sales = await _service.GetAllCustomers(1, 10);
                CustomerDataGrid.ItemsSource = sales.Where(s => s.CustomerName.ToLower().Contains(searchTerm));
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer selected = CustomerDataGrid.SelectedItem as Customer;
            if (selected != null)
            {
                MessageBox.Show("Vui lòng chọn 1 Khách hàng để xóa!", "Chọn khách hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CustomerService service = new();
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                service.DeleteCustomer(selected.CustomerId);
                MessageBox.Show("Đã xóa khách hàng thành công", "Thành công", MessageBoxButton.OK);
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var customers = await _service.GetAllCustomers(_currentPage, _pageSize);
                var filteredCustomers = customers.Where(c => c.Status == 1).ToList();

                if (filteredCustomers.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Customers Data");

                    var header = new List<string> { "Số thứ tự", "Tên khách hàng", "Số điện thoại", "Số lượng RFID" };

                    for (int i = 0; i < header.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = header[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i + 1].AutoFitColumns();
                    }

                    for (int i = 0; i < filteredCustomers.Count; i++)
                    {
                        var customer = filteredCustomers[i];

                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = customer.CustomerName;
                        worksheet.Cells[i + 2, 3].Value = customer.Phone;
                        worksheet.Cells[i + 2, 4].Value = customer.RFIDCount;
                    }

                    for (int col = 1; col <= 4; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CaoSuData");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, $"CustomersData_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                    File.WriteAllBytes(filePath, package.GetAsByteArray());

                    MessageBox.Show($"Xuất file Excel thành công tại: {filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình xuất file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Error exporting Customers to Excel");
            }
        }

    }
}
