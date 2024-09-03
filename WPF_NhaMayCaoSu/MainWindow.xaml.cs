using MQTTnet.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using System.Windows.Media.Media3D;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MqttServerService _mqttServerService;
        private readonly MqttClientService _mqttClientService;
        private readonly CameraService _cameraService = new();
        public Account CurrentAccount { get; set; } = null;
        private BrokerWindow broker;
        private List<Sale> _sessionSaleList { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            broker = new BrokerWindow();
            _mqttServerService = MqttServerService.Instance;
            _mqttServerService.BrokerStatusChanged += (sender, e) => UpdateMainWindowUI();
            _mqttClientService = new MqttClientService();
            _mqttServerService.DeviceCountChanged += OnDeviceCountChanged;
            UpdateMainWindowUI();

        }

        private void OnSalesDataUpdated(object sender, List<Sale> updatedSales)
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine(updatedSales);
                SalesDataGrid.ItemsSource = null;
                SalesDataGrid.ItemsSource = updatedSales;
            });
        }
        private void UpdateMainWindowUI()
        {
            if (MqttServerService.IsBrokerRunning)
            {
                OpenServerButton.Content = Constants.CloseServerText;
                ServerStatusTextBlock.Text = Constants.ServerOnlineStatus;
                int deviceCount = _mqttServerService.GetDeviceCount();
                NumberofconnectionTextBlock.Text = $"Onl: {deviceCount} Thiết bị";
            }
            else
            {
                OpenServerButton.Content = Constants.OpenServerText;
                ServerStatusTextBlock.Text = Constants.ServerOfflineStatus;
            }
        }

        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("CreateRFID:"))
                {
                    string rfidString = data.Substring("CreateRFID:".Length);

                    if (!string.IsNullOrEmpty(rfidString))
                    {
                        SalesDataGrid.Dispatcher.Invoke(() =>
                        {
                            _sessionSaleList.Add(new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidString,
                                LastEditedTime = DateTime.Now,
                                Status = 1
                            });
                            Debug.WriteLine("Sale:" + _sessionSaleList);
                            SalesDataGrid.ItemsSource = null;
                            SalesDataGrid.ItemsSource = _sessionSaleList;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Failed to parse RFID number.");
                    }
                }
                else if (data.StartsWith("Can_ta:"))
                {
                    string[] parts = data.Substring("Can_ta:".Length).Split(':');
                    if (parts.Length == 2 && float.TryParse(parts[1], out float weight))
                    {
                        string rfidString = parts[0];

                        SalesDataGrid.Dispatcher.Invoke(() =>
                        {
                            _sessionSaleList.Add(new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidString,
                                ProductWeight = weight,
                                LastEditedTime = DateTime.Now,
                                Status = 1
                            });
                            Debug.WriteLine("Sale:" + _sessionSaleList);
                            SalesDataGrid.ItemsSource = null;
                            SalesDataGrid.ItemsSource = _sessionSaleList;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Failed to parse Can_ta data.");
                    }
                }
                else if (data.StartsWith("Can_tieu_ly:"))
                {
                    string[] parts = data.Substring("Can_tieu_ly:".Length).Split(':');
                    if (parts.Length == 2 && float.TryParse(parts[1], out float density))
                    {
                        string rfidString = parts[0];

                        SalesDataGrid.Dispatcher.Invoke(() =>
                        {
                            _sessionSaleList.Add(new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidString,
                                ProductDensity = density,
                                LastEditedTime = DateTime.Now,
                                Status = 1
                            });
                            Debug.WriteLine("Sale:" + _sessionSaleList);
                            SalesDataGrid.ItemsSource = null;
                            SalesDataGrid.ItemsSource = _sessionSaleList;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Failed to parse Can_tieu_ly data.");
                    }
                }
                else
                {
                    Debug.WriteLine("Unknown message format.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("hành động này sẽ đóng ứng dụng, bạn chắc chứ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            { 
                App.Current.Shutdown();
            }    
            
        }


        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void SaleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            SaleListWindow saleListWindow = new SaleListWindow();
            saleListWindow.CurrentAccount = CurrentAccount;
            Close();
            saleListWindow.ShowDialog();
        }


        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            Close();
            accountManagementWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
        }

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            broker.Show();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bạn đang ở cửa sổ hiển thị!", "Lặp cửa sổ!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Broker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MqttServerService.IsBrokerRunning)
                {
                    await _mqttServerService.StopBrokerAsync();
                    await _mqttClientService.CloseConnectionAsync();
                }
                else
                {
                    await _mqttServerService.StartBrokerAsync();
                    await _mqttClientService.ConnectAsync();
                }
                UpdateMainWindowUI();
            }
            catch (Exception ex)
            {
                if (OpenServerButton.Content.ToString() == Constants.OpenServerText)
                {
                    MessageBox.Show(Constants.BrokerStartErrorMessage + "\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(Constants.BrokerStopErrorMessage + "\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnDeviceCountChanged(object sender, int deviceCount)
        {
            Dispatcher.Invoke(() =>
            {
                NumberofconnectionTextBlock.Text = $"Onl:{deviceCount} Thiết bị";
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _mqttClientService.ConnectAsync();
            await _mqttClientService.SubscribeAsync("CreateRFID");
            await _mqttClientService.SubscribeAsync("Can_ta");
            await _mqttClientService.SubscribeAsync("Can_tieu_ly");
            _mqttClientService.MessageReceived += OnMqttMessageReceived;
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            roleListWindow.ShowDialog();
        }
        private async void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;
            }
            int choice = 0;
            Sale scannedSale = new();
            if (SalesDataGrid.SelectedItem is Sale selectedSale)
            {
                scannedSale = selectedSale;
                if (selectedSale.ProductWeight == null && selectedSale.ProductDensity == null)
                {
                    choice = 1;
                }
                else if (selectedSale.ProductDensity == null && selectedSale.ProductWeight != null)
                {
                    choice = 2;
                }
                else if (selectedSale.ProductWeight == null && selectedSale.ProductDensity != null)
                {
                    choice = 3;
                }
            }
            switch (choice)
            {
                case 1:
                    CreateRFID(scannedSale);
                    break;
                case 2:
                    AddWeight(scannedSale);
                    break;
                case 3:
                    AddDensity(scannedSale);
                    break;
                default:
                    break;
            }
        }
        private void CreateRFID(Sale sale)
        {
            RFIDManagementWindow window = new();
            window.ShowDialog();
        }
        private async void AddWeight(Sale sale)
        {
            //Tạo 2 trường hợp if else cho 2 trường hợp: Tạo và cập nhật


            MessageBox.Show("Dữ liệu cân tạ cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }
        private async void AddDensity(Sale sale)
        {
            //function to automagically add Density to db
            //await.......

            MessageBox.Show("Dữ liệu cân tiểu li cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }
    }
}





