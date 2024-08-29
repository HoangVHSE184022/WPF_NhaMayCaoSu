using System.Windows;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMqttService _mqttService;
        private readonly ICameraService _cameraService;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMqttService mqttService, ICameraService cameraService)
        {
            InitializeComponent();
            _mqttService = mqttService;
            _cameraService = cameraService;
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rfidListWindow = new RFIDListWindow();
            rfidListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();
        }
        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            await _mqttService.SubscribeAsync("a");
        }

        private async void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            await _mqttService.PublishAsync("b", "your_message_here");
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