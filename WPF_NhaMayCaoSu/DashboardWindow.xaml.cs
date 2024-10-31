using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;
using Xceed.Wpf.Toolkit.Converters;



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
        private List<Customer> allCustomers;
        private Customer _currentCustomer;
        private readonly MqttClientService _mqttClientService = new MqttClientService();
        private MainWindow _mainWindow;
        public Account CurrentAccount { get; set; } = null;

        public DashboardWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            FromDatePicker.SelectedDate = DateTime.Now.AddMonths(-1);
            ToDatePicker.SelectedDate = DateTime.Now;
            _mainWindow = mainWindow;

        }

        private async void LoadCustomersName()
        {
            try
            {
                allCustomers = (await _customerService.GetAllCustomersNoPagination()).ToList();
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
            string customerSearchText = CustomerTextBox.Text.ToLower();

            if (_salesData == null || !_salesData.Any())
                return;

            var filteredSales = _salesData.AsEnumerable();

            if (fromDate.HasValue)
                filteredSales = filteredSales.Where(s => s.LastEditedTime >= fromDate.Value.Date);

            if (toDate.HasValue)
                filteredSales = filteredSales.Where(s => s.LastEditedTime <= toDate.Value.Date.AddDays(1).AddTicks(-1));

            //string tag = TypeComboBox.SelectedValue.ToString();
            string tag = "";
            if (!string.IsNullOrEmpty(customerSearchText))
            {
                if (TypeComboBox.SelectedItem is ComboBoxItem selectedType)
                {
                    tag = selectedType.Tag.ToString();
                }
                switch (tag)
                {
                    case "Name":
                        filteredSales = filteredSales.Where(s => s.CustomerName.ToLower().Contains(customerSearchText));
                        break;
                    case "Phone":
                        Customer phoneCus = await _customerService.GetCustomerByPhoneAsync(customerSearchText);
                        if (phoneCus == null)
                        {
                            MessageBox.Show("Không tìm được khách hàng với Số điện thoại trên.", "Không tìm thấy", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        filteredSales = filteredSales.Where(s => s.CustomerName.ToLower().Equals(phoneCus.CustomerName.ToLower()));
                        break;
                    case "RFID":
                        filteredSales = filteredSales.Where(s => s.RFIDCode.ToLower().Contains(customerSearchText));
                        break;
                    default:
                        filteredSales = filteredSales.Where(s => s.CustomerName.ToLower().Contains(customerSearchText));
                        break;
                }
            }

            if (FilterByZeroComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedProperty = selectedItem.Tag.ToString();

                if (!string.IsNullOrEmpty(selectedProperty))
                {
                    filteredSales = filteredSales.Where(s =>
                    {
                        var propertyValue = typeof(Sale).GetProperty(selectedProperty)?.GetValue(s);
                        return propertyValue == null || propertyValue.ToString() == "0";
                    });
                }
            }

            _filteredSalesData = filteredSales.ToList();

            var paginatedSales = _filteredSalesData.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            SalesDataGrid.ItemsSource = paginatedSales;

            UpdateStatistics(_filteredSalesData);
            _totalPages = (int)Math.Ceiling((double)_filteredSalesData.Count() / _pageSize);
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
            //LoadCustomers();

            LoadSalesData();

            LoadCustomersName();
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

        private void CustomerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = CustomerTextBox.Text.ToLower();
            string tag = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag.ToString();
            var filteredSuggestions = allCustomers;

            if (tag == "Name")
            {
                filteredSuggestions = allCustomers
                    .Where(c => c.CustomerName.ToLower().Contains(query))
                    .ToList();
                SuggestionListBox.DisplayMemberPath = "CustomerName";
            }
            else if (tag == "Phone")
            {
                filteredSuggestions = allCustomers
                    .Where(c => c.Phone.ToLower().Contains(query))
                    .ToList();
                SuggestionListBox.DisplayMemberPath = "Phone";
            }

            SuggestionListBox.ItemsSource = filteredSuggestions;
            SuggestionPopup.IsOpen = filteredSuggestions.Any();
        }


        private void SuggestionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuggestionListBox.SelectedItem is Customer selectedCustomer)
            {
                _currentCustomer = selectedCustomer;
                CustomerTextBox.Text = selectedCustomer.CustomerName;
                SuggestionPopup.IsOpen = false;
                FilterSalesData();
            }
        }

        private void CustomerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //string query = CustomerTextBox.Text.ToLower();
                //string tag = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag.ToString();

                //if (tag == "Name")
                //{
                //    _currentCustomer = allCustomers.FirstOrDefault(c => c.CustomerName.ToLower().Contains(query));
                //    Debug.WriteLine(_currentCustomer);
                //}
                //else if (tag == "Phone")
                //{
                //    _currentCustomer = allCustomers.FirstOrDefault(c => c.Phone.ToLower().Contains(query));
                //}

                SuggestionPopup.IsOpen = false;
                FilterSalesData();
            }
        }

        private void FilterByZeroComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesData();
        }

        private void OpenEditSaleWindow(object sender, RoutedEventArgs e)
        {
            Sale selectedSale = ((Button)sender).Tag as Sale;
            if (selectedSale == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch để chỉnh sửa.", "Không có giao dịch được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saleManagementWindow = new SaleManagementWindow(_mqttClientService, _mainWindow)
            {
                SelectedSale = selectedSale,
                CurrentAccount = CurrentAccount
            };


            saleManagementWindow.ShowDialog();
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesData();
        }
    }
}

