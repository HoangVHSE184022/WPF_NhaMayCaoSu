using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        private List<Sale> _salesData;

        public DashboardWindow()
        {
            InitializeComponent();
            FromDatePicker.SelectedDate = DateTime.Now;
            ToDatePicker.SelectedDate = DateTime.Now;

        }

        private async void LoadCustomers()
        {
            try
            {
                // Lấy danh sách khách hàng từ service
                var customers = await _customerService.GetAllCustomersNoPagination();

                // Thêm tùy chọn "Tất cả"
                var allCustomersOption = new Customer
                {
                    CustomerId = Guid.Empty,  // Sử dụng GUID rỗng để đại diện cho "Tất cả"
                    CustomerName = "Tất cả"
                };

                // Nối "Tất cả" với danh sách khách hàng
                var customerList = new[] { allCustomersOption }.Concat(customers);

                // Gán danh sách khách hàng vào ComboBox
                CustomerComboBox.ItemsSource = customerList;
                CustomerComboBox.DisplayMemberPath = "CustomerName"; // Hiển thị tên khách hàng
                CustomerComboBox.SelectedValuePath = "CustomerId";   // Lấy giá trị là CustomerId

                // Đặt giá trị mặc định cho ComboBox
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

                // Lấy dữ liệu bán hàng từ service
                _salesData = allSales.ToList();

                // Gán dữ liệu cho DataGrid
                SalesDataGrid.ItemsSource = _salesData;

                // Cập nhật thống kê tổng quan
                UpdateStatistics();
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

            // Nếu không có dữ liệu, thoát
            if (_salesData == null || !_salesData.Any())
            {
                return;
            }
            IEnumerable<Sale> fetchSalesData = await _saleService.GetAllSaleAsync();
            if (fetchSalesData == null || !fetchSalesData.Any())
            {
                return;
            }
            List<Sale> filteredSales = fetchSalesData.ToList();

            // Nếu chọn "Tất cả", lấy toàn bộ dữ liệu bán hàng
            if (selectedCustomer != null && selectedCustomer.CustomerId == Guid.Empty)
            {
                filteredSales.ToList();
            }
            else
            {
                // Lọc dữ liệu bán hàng theo ngày và khách hàng
                filteredSales = _salesData;

                // Lọc theo ngày bắt đầu
                if (fromDate.HasValue)
                {
                    // Thiết lập giờ cho ngày bắt đầu
                    DateTime startDate = fromDate.Value.Date; // Ngày bắt đầu sẽ là 00:00:00
                    filteredSales = filteredSales.Where(s => s.LastEditedTime >= startDate).ToList();
                }

                // Lọc theo ngày kết thúc
                if (toDate.HasValue)
                {
                    // Thiết lập giờ cho ngày kết thúc
                    DateTime endDate = toDate.Value.Date.AddDays(1).AddTicks(-1); // Đến 23:59:59.9999999
                    filteredSales = filteredSales.Where(s => s.LastEditedTime <= endDate).ToList();
                }

                if (selectedCustomer != null)
                {
                    filteredSales = filteredSales.Where(s => s.CustomerName.ToLower() == selectedCustomer.CustomerName.ToLower()).ToList();
                }
            }

            // Cập nhật DataGrid và thống kê
            SalesDataGrid.ItemsSource = null;
            SalesDataGrid.ItemsSource = filteredSales.ToList();
            UpdateStatistics(filteredSales.ToList());
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
    }
}
