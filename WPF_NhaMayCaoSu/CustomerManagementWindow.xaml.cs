﻿using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Service.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerManagementWindow.xaml
    /// </summary>
    public partial class CustomerManagementWindow : Window
    {
        private CustomerService _service = new();
        private MqttClientService _mqttClientService = new MqttClientService();

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
                string.IsNullOrWhiteSpace(RFIDCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(StatusTextBox.Text))
            {
                MessageBox.Show(Constants.ErrorMessageMissingFields, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Status (ensure it's either 0 or 1)
            if (!short.TryParse(StatusTextBox.Text, out short status) || (status != 0 && status != 1))
            {
                MessageBox.Show(Constants.ErrorMessageInvalidStatus, Constants.ErrorTitleValidation, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Proceed to create or update the customer
            Customer customer = new()
            {
                CustomerName = AccountNameTextBox.Text,
                RFIDCode = long.TryParse(RFIDCodeTextBox.Text, out long rfidCode) ? rfidCode : 0,
                Status = status,
                CreatedDate = SelectedCustomer == null ? DateTime.Now : SelectedCustomer.CreatedDate,
                ExpirationDate = SelectedCustomer == null ? DateTime.Now.AddDays(1) : SelectedCustomer.ExpirationDate,
                CustomerId = SelectedCustomer?.CustomerId ?? Guid.NewGuid()
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
            await _mqttClientService.ConnectAsync();
            await _mqttClientService.SubscribeAsync("CreateRFID");

            _mqttClientService.MessageReceived += OnMqttMessageReceived;

            if (SelectedCustomer != null)
            {
                AccountNameTextBox.Text = SelectedCustomer.CustomerName;
                RFIDCodeTextBox.Text = SelectedCustomer.RFIDCode.ToString();
                StatusTextBox.Text = SelectedCustomer.Status.ToString();
                ModeLabel.Content = Constants.ModeLabelEditCustomer;
            }
        }


        private void OnMqttMessageReceived(object sender, string data)
        {
            try
            {
                if (data.StartsWith("CreateRFID:"))
                {
                    string rfidString = data.Substring("CreateRFID:".Length);

                    if (long.TryParse(rfidString, out long rfid))
                    {
                        RFIDCodeTextBox.Dispatcher.Invoke(() =>
                        {
                            RFIDCodeTextBox.Text = rfid.ToString();
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Failed to parse RFID number.");
                    }
                }
                else
                {
                    Debug.WriteLine("Unexpected message format.");
                }
            }
            catch (Exception ex)
            {
                // Handle any general errors
                Debug.WriteLine($"Error processing message: {ex.Message}");
            }
        }

    }
}
