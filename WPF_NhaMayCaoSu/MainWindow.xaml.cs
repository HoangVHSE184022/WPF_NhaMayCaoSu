using System.Windows;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMqttService _mqttService;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMqttService mqttService)
        {
            InitializeComponent();
            _mqttService = mqttService;
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

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();
        }

        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.ShowDialog();
        }
        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            await _mqttService.SubscribeAsync("a");
        }

        private async void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            await _mqttService.PublishAsync("b", "your_message_here");
        }
    }
}