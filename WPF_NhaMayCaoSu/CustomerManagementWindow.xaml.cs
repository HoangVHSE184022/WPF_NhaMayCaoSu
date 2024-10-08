﻿using System.Windows;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerManagementWindow.xaml
    /// </summary>
    public partial class CustomerManagementWindow : Window
    {
        private ICustomerService _service = new CustomerService();
        private MqttClientService _mqttClientService = new MqttClientService();
        public Account CurrentAccount { get; set; } = null;

        public Customer SelectedCustomer { get; set; } = null;

        public CustomerManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show(Constants.ErrorMessageMissingFields, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            // Proceed to create or update the customer
            Customer customer = new()
            {
                CustomerName = AccountNameTextBox.Text,
                Status = 1,
                CustomerId = SelectedCustomer?.CustomerId ?? Guid.NewGuid(),
                Phone = PhoneTextBox.Text
            };

            if (SelectedCustomer == null)
            {
                await _service.CreateCustomer(customer);
                MessageBox.Show(string.Format(Constants.SuccessMessageCreateCustomer, customer.CustomerName), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await _service.UpdateCustomer(customer);
                MessageBox.Show(string.Format(Constants.SuccessMessageUpdateCustomer, customer.CustomerName), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = Constants.ModeLabelAddCustomer;
            //await _mqttClientService.ConnectAsync();
            //await _mqttClientService.SubscribeAsync("CreateRFID");

            //_mqttClientService.MessageReceived += OnMqttMessageReceived;

            if (SelectedCustomer != null)
            {
                AccountNameTextBox.Text = SelectedCustomer.CustomerName;
                PhoneTextBox.Text = SelectedCustomer.Phone.ToString();
                ModeLabel.Content = Constants.ModeLabelEditCustomer;
            }
        }


        //private void OnMqttMessageReceived(object sender, string data)
        //{
        //    try
        //    {
        //        if (data.StartsWith("CreateRFID:"))
        //        {
        //            string rfidString = data.Substring("CreateRFID:".Length);

        //            if (!rfidString.IsNullOrEmpty())
        //            {
        //                RFIDCodeTextBox.Dispatcher.Invoke(() =>
        //                {
        //                    RFIDCodeTextBox.Text = rfidString;
        //                });
        //            }
        //            else
        //            {
        //                Debug.WriteLine("Failed to parse RFID number.");
        //            }
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Unexpected message format.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any general errors
        //        Debug.WriteLine($"Error processing message: {ex.Message}");
        //    }
        //}

        private void CustomerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerListWindow customerListWindow = new CustomerListWindow();
            customerListWindow.CurrentAccount = CurrentAccount;
            Close();
            customerListWindow.ShowDialog();
        }

        private void RFIDManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDListWindow rFIDListWindow = new RFIDListWindow();
            rFIDListWindow.CurrentAccount = CurrentAccount;
            Close();
            rFIDListWindow.ShowDialog();
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

        private void BrokerManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BrokerWindow brokerWindow = new BrokerWindow();
            brokerWindow.CurrentAccount = CurrentAccount;
            Close();
            brokerWindow.ShowDialog();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigCamera configCamera = new ConfigCamera();
            configCamera.ShowDialog();
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.CurrentAccount = CurrentAccount;
            Close();
            mainWindow.Show();
        }

        private void RoleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoleListWindow roleListWindow = new();
            roleListWindow.CurrentAccount = CurrentAccount;
            Close();
            roleListWindow.ShowDialog();
        }

    }
}
