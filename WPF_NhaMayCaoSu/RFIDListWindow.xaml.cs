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
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDListWindow.xaml
    /// </summary>
    public partial class RFIDListWindow : Window
    {
        public Account CurrentAccount { get; set; } = null;
        private RFIDService _service = new();
        public RFIDListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void LoadDataGrid()
        {
            RFIDDataGrid.ItemsSource = null;
            RFIDDataGrid.Items.Clear();
            RFIDDataGrid.ItemsSource = await _service.GetAllRFIDsAsync(1, 10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.CurrentAccount = CurrentAccount;
            mainWindow.Show();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }

        private void EditRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rFIDManagementWindow = new RFIDManagementWindow();
            rFIDManagementWindow.SelectedRFID = RFIDDataGrid.SelectedItem as RFID;
            rFIDManagementWindow.ShowDialog();
            LoadDataGrid();
        }
    }
}
