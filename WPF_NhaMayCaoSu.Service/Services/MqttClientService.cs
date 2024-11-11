using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class MqttClientService : IMqttClientService
    {
        public readonly IMqttClient _client;
        private MqttClientOptionsBuilder _optionsBuilder;
        private MqttClientOptions _options;
        private string _currentIp;
        private IBoardService _service = new BoardService();

        public event EventHandler<string> MessageReceived;
        public bool IsConnected => _client?.IsConnected ?? false;

        public MqttClientService()
        {
            MqttFactory factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _optionsBuilder = new MqttClientOptionsBuilder();
            _currentIp = GetLocalIpAddress();
            SetMqttOptions(_currentIp);

            // Register event handlers for MQTT
            _client.ConnectedAsync += OnConnectedAsync;
            _client.DisconnectedAsync += OnDisconnectedAsync;
            _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

            // Listen to network address changes
            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
        }

        private string GetLocalIpAddress()
        {
            System.Net.IPAddress[] ipAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in ipAddresses)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "N/A";
        }

        private void SetMqttOptions(string ipAddress)
        {
            _options = _optionsBuilder.WithClientId("this_computer")
                                      .WithTcpServer(ipAddress, 1883)
                                      .WithCredentials("admin", "admin")
                                      .WithCleanSession()
                                      .Build();
        }

        private async void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            string newIp = GetLocalIpAddress();
            if (_currentIp != newIp)
            {
                _currentIp = newIp;
                Debug.WriteLine($"Network address changed. New IP: {_currentIp}");

                // Disconnect from the current MQTT session
                if (_client.IsConnected)
                {
                    await _client.DisconnectAsync();
                    Debug.WriteLine("Disconnected from MQTT due to network change.");
                }

                // Update MQTT options with the new IP address
                SetMqttOptions(_currentIp);

                // Reconnect to the MQTT broker with the new IP
                await ReconnectAsync();
            }
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
            Debug.WriteLine($"Message received on topic {arg.ApplicationMessage.Topic}: {message}");

            var jsonMessage = JsonConvert.DeserializeObject<JObject>(message);
            string rfid = jsonMessage["RFID"]?.ToString();
            string density = jsonMessage["Density"]?.ToString();
            string weight = jsonMessage["Weight"]?.ToString();
            string data = jsonMessage["Data"]?.ToString();
            string macAddress = jsonMessage["MacAddress"]?.ToString();
            string Mode = jsonMessage["Mode"]?.ToString();

            switch (arg.ApplicationMessage.Topic)
            {
                case var topic when topic.EndsWith("/sendRFID"):
                    HandleSendRFID(arg.ApplicationMessage.Topic, rfid);
                    break;

                case var topic when topic.EndsWith("/info"):
                    HandleInfoTopic(rfid, data, topic);
                    break;

                case var topic when topic.EndsWith("/checkmode"):
                    HandleCheckMode(arg.ApplicationMessage.Topic, Mode);
                    break;

                default:
                    Debug.WriteLine("Unexpected topic received");
                    break;
            }
            return Task.CompletedTask;
        }

        private void HandleSendRFID(string topic, string rfid)
        {
            string[] topicParts = topic.Split('/');
            if (topicParts.Length > 1 && !string.IsNullOrEmpty(rfid))
            {
                string macAddress = topicParts[0];
                MessageReceived?.Invoke(this, $"sendRFID-{macAddress}-{rfid}");
            }
        }

        private async void HandleInfoTopic(string rfid, string data, string topic)
        {
            string[] topicParts = topic.Split('/');
            if (topicParts.Length > 1 && !string.IsNullOrEmpty(rfid))
            {
                string macAddress = topicParts[0];
                Board _board = await _service.GetBoardByMacAddressAsync(macAddress);

                if (_board == null)
                {
                    Debug.WriteLine($"Cannot found board {_board}");
                    return;
                }
                if (_board.BoardName == "Cân Tạ")
                {
                    MessageReceived?.Invoke(this, $"info-{rfid}-{data}-Weight-{macAddress}");
                }
                else if (_board.BoardName == "Cân Tiểu Ly")
                {
                    MessageReceived?.Invoke(this, $"info-{rfid}-{data}-Density-{macAddress}");
                }
                else
                {
                    Debug.WriteLine("Info message received but cannot found board");
                }
            }
            else
            {
                Debug.WriteLine("Info message received but RFID is missing");
            }
        }

        private void HandleCheckMode(string topic, string mode)
        {
            string[] topicParts = topic.Split('/');
            if (topicParts.Length > 1 && !string.IsNullOrEmpty(mode))
            {
                string mac = topicParts[0];
                MessageReceived?.Invoke(this, $"checkmode-{mac}-{mode}");
            }
        }

        public async Task ConnectAsync()
        {
            if (!_client.IsConnected)
            {
                try
                {
                    await _client.ConnectAsync(_options);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to connect to MQTT broker: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task SubscribeAsync(string topic)
        {
            var topicFilter = new MqttTopicFilterBuilder().WithTopic(topic).Build();
            await _client.SubscribeAsync(topicFilter);
        }

        public async Task PublishAsync(string topic, string payload)
        {
            if (!_client.IsConnected)
            {
                try
                {
                    await _client.ConnectAsync(_options);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to connect to MQTT broker before publishing: " + ex.Message);
                    throw;
                }
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();

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

        private async Task ReconnectAsync()
        {
            try
            {
                await _client.ConnectAsync(_options);
                Debug.WriteLine($"Reconnected to MQTT with new IP: {_currentIp}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to reconnect to MQTT: {ex.Message}");
            }
        }
    }
}
