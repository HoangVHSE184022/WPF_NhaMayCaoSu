using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {

        private readonly ISaleService _saleService = new SaleService();
        private readonly ICustomerService _customerService = new CustomerService();
        private readonly IImageService _imageService = new ImageService();
        private List<Sale> _salesData;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages;
        private List<Sale> _filteredSalesData;

        public DashboardWindow()
        {
            InitializeComponent();
            FromDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            ToDatePicker.SelectedDate = DateTime.Now;

        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersNoPagination();

                var allCustomersOption = new Customer
                {
                    CustomerId = Guid.Empty,
                    CustomerName = "Tất cả"
                };

                var customerList = new[] { allCustomersOption }.Concat(customers);

                CustomerComboBox.ItemsSource = customerList;
                CustomerComboBox.DisplayMemberPath = "CustomerName";
                CustomerComboBox.SelectedValuePath = "CustomerId";

                CustomerComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tải danh sách khách hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadSalesData()
        {
            try
            {
                IEnumerable<Sale> allSales = await _saleService.GetAllSaleAsync();
                _salesData = allSales.ToList();
                FilterSalesData(); // Apply filtering and pagination on load

                // Update pagination controls
                UpdatePaginationControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi tải dữ liệu bán hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesData();

        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesData();

        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesData();
        }

        private async void FilterSalesData()
        {
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            Customer selectedCustomer = CustomerComboBox.SelectedItem as Customer;

            // If no data available, exit
            if (_salesData == null || !_salesData.Any())
                return;

            // Filter based on date and customer selection
            var filteredSales = _salesData.AsEnumerable();

            if (fromDate.HasValue)
                filteredSales = filteredSales.Where(s => s.LastEditedTime >= fromDate.Value.Date);

            if (toDate.HasValue)
                filteredSales = filteredSales.Where(s => s.LastEditedTime <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            if (selectedCustomer != null && selectedCustomer.CustomerId != Guid.Empty)
                filteredSales = filteredSales.Where(s => s.CustomerName.ToLower() == selectedCustomer.CustomerName.ToLower());

            _filteredSalesData = filteredSales.ToList();

            // Paginate filtered data
            var paginatedSales = filteredSales.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();

            // Update DataGrid
            SalesDataGrid.ItemsSource = paginatedSales;
            UpdateStatistics(filteredSales.ToList());

            // Update pagination control based on filtered data
            _totalPages = (int)Math.Ceiling((double)filteredSales.Count() / _pageSize);
            UpdatePaginationControls();
        }

        private void UpdatePaginationControls()
        {
            PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
            PreviousPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }


        private void UpdateStatistics(List<Sale> salesData = null)
        {
            var data = salesData ?? _salesData;

            TotalWeight.Text = data.Sum(s => s.ProductWeight).ToString();
            TotalAmount.Text = data.Count().ToString();
            TotalTotalPrice.Text = data.Sum(s => s.TotalPrice).ToString();
        }



        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomers();

            LoadSalesData();
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }

        private async void ExportDataGridToExcel()
        {
            try
            {
                if (_filteredSalesData == null || _filteredSalesData.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sales Data");
                    var header = new List<string> { "Số thứ tự", "SaleId", "Tên khách hàng", "Số ký", "Tỉ trọng", "Số bì", "Lần chỉnh sửa cuối", "Mã RFID", "Đơn giá", "Giá thêm", "Tổng tiền", "Hình ảnh (Tỉ trọng)", "Hình ảnh (Cân nặng)" };

                    for (int i = 0; i < header.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = header[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, i + 1].AutoFitColumns();
                    }

                    for (int i = 0; i < _filteredSalesData.Count; i++)
                    {
                        var sale = _filteredSalesData[i];
                        var images = await _imageService.GetImagesBySaleIdAsync(sale.SaleId);
                        string densityImageUrl = images.FirstOrDefault(img => img.ImageType == 2)?.ImagePath ?? "N/A";
                        string weightImageUrl = images.FirstOrDefault(img => img.ImageType == 1)?.ImagePath ?? "N/A";

                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = sale.SaleId;
                        worksheet.Cells[i + 2, 3].Value = sale.CustomerName;
                        worksheet.Cells[i + 2, 4].Value = sale.ProductWeight;
                        worksheet.Cells[i + 2, 5].Value = sale.ProductDensity;
                        worksheet.Cells[i + 2, 6].Value = sale.TareWeight;
                        worksheet.Cells[i + 2, 7].Value = sale.LastEditedTime.HasValue ? sale.LastEditedTime.Value.ToString("g") : "N/A";
                        worksheet.Cells[i + 2, 8].Value = sale.RFIDCode;
                        worksheet.Cells[i + 2, 9].Value = sale.SalePrice;
                        worksheet.Cells[i + 2, 10].Value = sale.BonusPrice;
                        worksheet.Cells[i + 2, 11].Value = sale.TotalPrice;
                        worksheet.Cells[i + 2, 12].Value = densityImageUrl;
                        worksheet.Cells[i + 2, 13].Value = weightImageUrl;
                    }

                    for (int col = 1; col <= 14; col++)
                        worksheet.Column(col).AutoFit();

                    string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CaoSuData");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string filePath = Path.Combine(folderPath, $"SalesDataFromDashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());

                    MessageBox.Show($"Xuất file Excel thành công tại: {filePath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình xuất file Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportDataGridToExcel();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadSalesData();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadSalesData();
            }
        }
    }
}
