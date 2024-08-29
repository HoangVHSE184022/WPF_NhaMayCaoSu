using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleManagementWindow.xaml
    /// </summary>
    public partial class SaleManagementWindow : Window
    {

        private SaleService _service = new();

        public Sale SelectedSale { get; set; } = null;

        public SaleManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Sale x = new();

            x.ProductDensity = Double.Parse(DensityTextBox.Text);
            x.DensityImageUrl = URLTextBox.Text;
            x.Status = short.Parse(StatusTextBox.Text);
            x.RFIDCode = long.Parse(RFIDCodeTextBox.Text);

            if (SelectedSale == null)
            {
                x.CreatedDate = DateTime.Now;
                x.IsEdited = false;
                x.LastEditedTime = null;


                MessageBox.Show($"Created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateSaleAsync(x);
            }
            else
            {
                x.SaleId = SelectedSale.SaleId;
                x.CreatedDate = SelectedSale.CreatedDate;
                x.IsEdited = true;
                x.LastEditedTime = DateTime.Now;
                MessageBox.Show($"Updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateSaleAsync(x);
            }

            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = "Thêm Sale mới";

            if (SelectedSale != null)
            {
                RFIDCodeTextBox.Text = SelectedSale.RFIDCode.ToString();
                DensityTextBox.Text = SelectedSale.ProductDensity.ToString();
                URLTextBox.Text = SelectedSale.DensityImageUrl.ToString();
                StatusTextBox.Text = SelectedSale.Status.ToString();
                ModeLabel.Content = "Chỉnh sửa Sale";
            }
        }
    }
}

