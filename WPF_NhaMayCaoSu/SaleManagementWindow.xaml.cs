using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleManagementWindow.xaml
    /// </summary>
    public partial class SaleManagementWindow : Window
    {

        private SaleService _service = new();

        public Account CurrentAccount { get; set; } = null;

        public Sale SelectedSale { get; set; } = null;

        public SaleManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Sale x = new();

            //x.CustomerName = long.Parse(CustomerNameTextBox.Text);
            x.ProductWeight = int.Parse(WeightTextBox.Text);
            x.ProductDensity = int.Parse(DensityTextBox.Text);
            x.Status = short.Parse(StatusTextBox.Text);
            x.RFIDCode = RFIDCodeTextBox.Text;


            CameraService cameraService = new CameraService();
            Camera newestCamera = await cameraService.GetNewestCameraAsync();
            string imageFilePath = string.Empty;


            if (SelectedSale == null)
            {
                MessageBox.Show(Constants.SuccessMessageSaleCreated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateSaleAsync(x);
            }
            else
            {
                x.SaleId = SelectedSale.SaleId;
                x.LastEditedTime = DateTime.Now;
                MessageBox.Show(Constants.SuccessMessageSaleUpdated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateSaleAsync(x);
            }

            Close();
        }


       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = Constants.ModeLabelAddSale; ;

            if (SelectedSale != null)
            {
                RFIDCodeTextBox.Text = SelectedSale.RFIDCode.ToString();
                WeightTextBox.Text = SelectedSale.ProductWeight.ToString();
                StatusTextBox.Text = SelectedSale.Status.ToString();
                ModeLabel.Content = Constants.ModeLabelEditSale;
            }
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            rFIDListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.CurrentAccount = CurrentAccount;
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.ShowDialog();
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
    }
}

