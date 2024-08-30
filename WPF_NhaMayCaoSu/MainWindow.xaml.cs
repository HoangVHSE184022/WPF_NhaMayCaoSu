using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMqttServerService _mqttService;
        private readonly CameraService _cameraService = new();
        private readonly SessionService _sessionService;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMqttServerService mqttService)
        {
            InitializeComponent();
            _mqttService = mqttService;
            _sessionService = new();
        }

        public void FoundEvent(Sale sale)
        {
            _sessionService.AddToSalelist(sale);
            SalesDataGrid.ItemsSource = null;
            SalesDataGrid.ItemsSource = _sessionService.GetAllSales();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            // Ensure that _cameraService is initialized
            if (_cameraService == null)
            {
                MessageBox.Show("Camera service is not available.");
                return;
            }
            */
            DualCameraWindow dualCameraWindow = new DualCameraWindow(_cameraService);
            dualCameraWindow.ShowDialog();
        }

       
    }
}