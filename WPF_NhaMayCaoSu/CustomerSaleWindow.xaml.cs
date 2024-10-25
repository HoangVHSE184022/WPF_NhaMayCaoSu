using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerSaleWindow.xaml
    /// </summary>
    public partial class CustomerSaleWindow : Window
    {
        private readonly Customer _selectedCustomer;
        private readonly IImageService _imageService = new ImageService();
        private readonly ISaleService _saleService = new SaleService();
        private List<Sale> _customerSales;

        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages;
        public CustomerSaleWindow(Customer selectedCustomer)
        {
            InitializeComponent();
            _selectedCustomer = selectedCustomer;
            LoadDataGrid();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private async void LoadDataGrid()
        {
            try
            {
                Title.Content = $"Thống kê Sale của khách hàng {_selectedCustomer.CustomerName}";

                IEnumerable<Sale> customerSales = await _saleService.GetSalesByCustomerIdAsync(_selectedCustomer.CustomerId, _currentPage, _pageSize);
                IEnumerable<Sale> customerSalesTotal = await _saleService.GetSalesByCustomerIdAsync(_selectedCustomer.CustomerId);
                foreach (Sale sale in customerSales)
                {
                    CalculateTotalPrice(sale);
                }
                SaleDataGrid.ItemsSource = customerSales;
                int totalSalesCount = customerSalesTotal.Count();
                _totalPages = (int)Math.Ceiling((double)totalSalesCount / _pageSize);
                PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
                PreviousPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages;
                UpdateTotalLabel(customerSalesTotal);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu sale: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void CalculateTotalPrice(Sale sale)
        {
            if (sale != null)
            {
                float productWeight = sale.ProductWeight ?? 0;
                float tareWeight = sale.TareWeight ?? 0;
                float productDensity = sale.ProductDensity ?? 0;
                float salePrice = sale.SalePrice ?? 0;
                float bonusPrice = sale.BonusPrice ?? 0;

                sale.TotalPrice = (productWeight - tareWeight) * productDensity * (salePrice + bonusPrice);
            }
        }

        private void UpdateTotalLabel(IEnumerable<Sale> sales)
        {
            float totalSum = sales.Sum(s => s.TotalPrice ?? 0);
            Total.Content = $"Tổng thành tiền: {totalSum:N0} VNĐ";
        }

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesByDateAsync();
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSalesByDateAsync();
        }

        private async Task FilterSalesByDateAsync()
        {
            IEnumerable<Sale> customerSales = await _saleService.GetSalesByCustomerIdAsync(_selectedCustomer.CustomerId);
            if (customerSales == null) return;

            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;

            DateTime? normalizedFromDate = fromDate?.Date;
            DateTime? normalizedToDate = toDate?.Date.AddDays(1).AddSeconds(-1);

            if (normalizedFromDate.HasValue && normalizedToDate.HasValue)
            {
                if (normalizedFromDate > normalizedToDate)
                {
                    MessageBox.Show($"Ngày sau phải lớn hơn ngày trước", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                IEnumerable<Sale> filteredSales = customerSales.Where(sale =>
                    sale.LastEditedTime >= normalizedFromDate && sale.LastEditedTime <= normalizedToDate).ToList();

                foreach (Sale sale in filteredSales)
                {
                    CalculateTotalPrice(sale);
                }
                SaleDataGrid.ItemsSource = filteredSales;
                int totalSalesCount = filteredSales.Count();
                _totalPages = (int)Math.Ceiling((double)totalSalesCount / _pageSize);
                PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
                PreviousPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages;
                UpdateTotalLabel(filteredSales);
            }
            else
            {
                SaleDataGrid.ItemsSource = customerSales;
            }
        }


        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            Sale selectedSale = SaleDataGrid.SelectedItem as Sale;
            if (selectedSale == null)
            {
                MessageBox.Show("Vui lòng chọn một giao dịch từ danh sách.", "Không có giao dịch được chọn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var images = await _imageService.Get2LatestImagesBySaleIdAsync(selectedSale.SaleId);
            if (!images.Any())
            {
                MessageBox.Show("Giao dịch này không có hình ảnh nào được liên kết để hiển thị.", "Không tìm thấy hình ảnh", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ViewImagesWindow viewImagesWindow = new ViewImagesWindow(selectedSale);
            viewImagesWindow.ShowDialog();
        }
    }
}
