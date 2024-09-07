using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

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
            _mqttClientService = new MqttClientService();
            
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
        

        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                Sale scannedSale = new Sale();
                int choice = 0;
                if (data.StartsWith("CreateRFID:"))
                {
                    string rfidString = data.Substring("CreateRFID:".Length);

                    if (!string.IsNullOrEmpty(rfidString))
                    {
                        SalesDataGrid.Dispatcher.Invoke(() =>
                        {
                            scannedSale = new Sale
                            {
                                SaleId = Guid.NewGuid(),
                                RFIDCode = rfidString,
                                LastEditedTime = DateTime.UtcNow,
                                Status = 1
                            };
                            _sessionSaleList.Add(scannedSale);
                            SalesDataGrid.ItemsSource = null;
                            SalesDataGrid.ItemsSource = _sessionSaleList;
                            choice = 1;
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
                                LastEditedTime = DateTime.UtcNow,
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
                                LastEditedTime = DateTime.UtcNow,
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Hành động này sẽ đóng ứng dụng, bạn chắc chứ?", "Thoát", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mqttClientService.ConnectAsync();
                await _mqttClientService.SubscribeAsync("CreateRFID");
                await _mqttClientService.SubscribeAsync("Can_ta");
                await _mqttClientService.SubscribeAsync("Can_tieu_ly");
                _mqttClientService.MessageReceived += OnMqttMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến máy chủ MQTT. Vui lòng kiểm tra lại kết nối. Bạn sẽ được chuyển về màn hình quản lý Broker.", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                BrokerWindow brokerWindow = new BrokerWindow();
                brokerWindow.ShowDialog();
                this.Close();
            }
        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
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
                    await AddWeight(scannedSale);
                    break;
                case 3:
                    await AddDensity(scannedSale);
                    break;
                default:
                    break;
            }
        }
        private void CreateRFID(Sale sale)
        {
            RFIDManagementWindow window = new RFIDManagementWindow(sale.RFIDCode);
            window.ShowDialog();
        }

        private async Task AddWeight(Sale sale)
        {
            MessageBox.Show("Dữ liệu cân tạ cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }
        private async Task AddDensity(Sale sale)
        {
            MessageBox.Show("Dữ liệu cân tiểu li cập nhật thành công", "Cập nhật dữ liệu", MessageBoxButton.OK);
        }

        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    this.Hide();
        //    e.Cancel = true;
        //}

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow viewWindow = new();
            viewWindow._sessionSaleList = this._sessionSaleList;
            viewWindow.CurrentAccount = this.CurrentAccount;

            viewWindow.WindowState = WindowState.Maximized;
            viewWindow.WindowStyle = WindowStyle.None;
            viewWindow.ResizeMode = ResizeMode.NoResize; 

            viewWindow.Show();
        }


    }
}





