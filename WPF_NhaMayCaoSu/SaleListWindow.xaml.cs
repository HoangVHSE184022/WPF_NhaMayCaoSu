using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {

        private SaleService _service = new();

        public SaleListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {

            Sale selected = SaleDataGrid.SelectedItem as Sale;

            if (selected == null)
            {
                MessageBox.Show(Constants.ErrorMessageSelectSale, Constants.ErrorTitleSelectSale, MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.SelectedSale = selected;
            saleManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private async void LoadDataGrid()
        {
            SaleDataGrid.ItemsSource = null;
            SaleDataGrid.Items.Clear();
            SaleDataGrid.ItemsSource = await _service.GetAllSaleAsync(1, 10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }
    }
}
