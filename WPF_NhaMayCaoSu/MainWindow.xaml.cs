using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IMqttService mqttService)
        {
            InitializeComponent();
            _mqttService = mqttService;
            _ = TestMqttConnection();
        }


     private async Task TestMqttConnection()
        {
            bool isConnected = await _mqttService.TestConnectionAsync();
            if (isConnected)
            {
                MessageBox.Show("Connected to MQTT broker successfully.");
            }
            else
            {
                MessageBox.Show("Failed to connect to MQTT broker.");
            }
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