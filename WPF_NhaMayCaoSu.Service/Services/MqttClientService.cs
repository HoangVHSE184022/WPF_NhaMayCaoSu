﻿using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using System.Text;
using WPF_NhaMayCaoSu.Service.Interfaces;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MQTTnet.Adapter;
using static System.Net.Mime.MediaTypeNames;
using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;


namespace WPF_NhaMayCaoSu.Service.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;

        public event EventHandler<string> MessageReceived;
        public bool IsConnected => _client?.IsConnected ?? false;
        public List<Sale> _sessionSaleList { get; private set; }
        public event EventHandler<List<Sale>> SalesDataUpdated;


        public MqttClientService()
        {
            MqttFactory factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder();
            string ipLocal = GetLocalIpAddress();
            optionsBuilder.WithClientId("this_computer")
                           .WithTcpServer($"{ipLocal}", 1883)
                           .WithCredentials("admin", "admin")
                           .WithCleanSession();

            _options = optionsBuilder.Build();

            // Register event handlers
            _client.ConnectedAsync += OnConnectedAsync;
            _client.DisconnectedAsync += OnDisconnectedAsync;
            _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        }

        private void UpdateDataGrid()
        {
            SalesDataUpdated?.Invoke(this, _sessionSaleList);
        }

        private string GetLocalIpAddress()
        {
            System.Net.IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            foreach (System.Net.IPAddress ip in ipAddresses)
            {
                // Check for IPv4 addresses
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "N/A";
        }

        private Task OnConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Debug.WriteLine("Connected to MQTT broker.");
            return Task.CompletedTask;
        }

        private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Debug.WriteLine("Disconnected from MQTT broker.");
            return Task.CompletedTask;
        }

        private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            string message = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Console.WriteLine($"Message received on topic {arg.ApplicationMessage.Topic}: {message}");

            var jsonMessage = JsonConvert.DeserializeObject<JObject>(message);
            string rfid = jsonMessage["RFID"]?.ToString();
            string density = jsonMessage["Density"]?.ToString();
            string weight = jsonMessage["Weight"]?.ToString();

            switch (arg.ApplicationMessage.Topic)
            {
                case "CreateRFID":
                    if (!string.IsNullOrEmpty(rfid))
                    {
                        _sessionSaleList.Add(new Sale
                        {
                            SaleId = Guid.NewGuid(),
                            RFIDCode = rfid,
                            LastEditedTime = DateTime.Now,
                            Status = 1 // Hoặc giá trị trạng thái khác
                        });
                        UpdateDataGrid();
                        MessageReceived?.Invoke(this, $"CreateRFID:{rfid}");
                    }
                    break;

                case "Can_tieu_ly":
                    if (!string.IsNullOrEmpty(rfid) && !string.IsNullOrEmpty(density))
                    {
                        _sessionSaleList.Add(new Sale
                        {
                            SaleId = Guid.NewGuid(),
                            RFIDCode = rfid,
                            ProductDensity = float.Parse(density),
                            LastEditedTime = DateTime.Now,
                            Status = 1
                        });
                        UpdateDataGrid();
                        MessageReceived?.Invoke(this, $"Can_tieu_ly:{rfid}:{density}");
                    }
                    break;

                case "Can_ta":
                    if (!string.IsNullOrEmpty(rfid) && !string.IsNullOrEmpty(weight))
                    {
                        _sessionSaleList.Add(new Sale
                        {
                            SaleId = Guid.NewGuid(),
                            RFIDCode = rfid,
                            ProductWeight = float.Parse(weight),
                            LastEditedTime = DateTime.Now,
                            Status = 1
                        });
                        UpdateDataGrid();
                        MessageReceived?.Invoke(this, $"Can_ta:{rfid}:{weight}");
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        //private void UpdateDataGrid()
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        var mainWindow = (MainWindow)Application.Current.MainWindow;
        //        mainWindow.SalesDataGrid.ItemsSource = null;
        //        mainWindow.SalesDataGrid.ItemsSource = _sessionSaleList;
        //    });
        //}


        public async Task ConnectAsync()
        {
            if (!_client.IsConnected)
            {
                try
                {
                    await _client.ConnectAsync(_options);
                }
                catch (MqttConnectingFailedException ex)
                {
                    Debug.WriteLine("Failed to connect to MQTT server: " + ex.Message);
                    throw new Exception("Failed to connect to MQTT server. Please ensure the server is turned on.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred while connecting to MQTT server: " + ex.Message);
                    throw new Exception("An error occurred while connecting to the MQTT server. Please ensure the server is turned on.", ex);
                }
            }
        }


        public async Task SubscribeAsync(string topic)
        {
            MqttTopicFilterBuilder topicFilterBuilder = new MqttTopicFilterBuilder();
            topicFilterBuilder.WithTopic(topic);

            await _client.SubscribeAsync(topicFilterBuilder.Build());
        }

        public async Task PublishAsync(string topic, string payload)
        {
            MqttApplicationMessageBuilder messageBuilder = new MqttApplicationMessageBuilder();
            messageBuilder.WithTopic(topic)
                          .WithPayload(payload)
                          .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                          .WithRetainFlag();

            MqttApplicationMessage message = messageBuilder.Build();

            await _client.PublishAsync(message);
        }

        public async Task CloseConnectionAsync()
        {
            if (_client.IsConnected)
            {
                await _client.DisconnectAsync();
                Console.WriteLine("Disconnected from MQTT broker.");
            }
        }
    }
}
