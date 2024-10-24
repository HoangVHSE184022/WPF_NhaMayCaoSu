using System;
using System.Collections.Generic;
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

        public DashboardWindow()
        {
            InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = DatePicker.SelectedDate.Value;
                await LoadSalesData(selectedDate);
            }
            else
            {
                MessageBox.Show("Please select a date.");
            }
        }

        // Method to load sales data for the selected date
        private async Task LoadSalesData(DateTime selectedDate)
        {
            // Load Sales Count by Day
            int salesCountByDay = await _saleService.GetSalesCountByDateAsync(selectedDate);
            SalesCountByDayLabel.Text = $"Số lượng Sale trong ngày {selectedDate:dd/MM/yyyy}:";
            SalesCountByDay.Text = salesCountByDay.ToString();

            // Load Sales Count by Month
            int salesCountByMonth = await _saleService.GetSalesCountByMonthAsync(selectedDate.Year, selectedDate.Month);
            SalesCountByMonthLabel.Text = $"Số lượng Sale trong tháng {selectedDate:MM/yyyy}:";
            SalesCountByMonth.Text = salesCountByMonth.ToString();

            // Load Sales Count by Year
            int salesCountByYear = await _saleService.GetSalesCountByYearAsync(selectedDate.Year);
            SalesCountByYearLabel.Text = $"Số lượng Sale trong năm {selectedDate:yyyy}:";
            SalesCountByYear.Text = salesCountByYear.ToString();
        }

        // DatePicker Changed Event
        private async void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = DatePicker.SelectedDate.Value;
                await LoadSalesData(selectedDate);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = DatePicker.SelectedDate.Value;
                await LoadSalesData(selectedDate);
            }

        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }
    }
}
