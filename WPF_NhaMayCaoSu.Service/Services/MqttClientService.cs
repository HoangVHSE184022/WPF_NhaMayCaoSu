using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using WPF_NhaMayCaoSu.Service.Interfaces;


namespace WPF_NhaMayCaoSu.Service.Services
{
    public class MqttClientService : IMqttClientService
    {
        private readonly IMqttClient _client;
        private readonly MqttClientOptions _options;

        public event EventHandler<string> MessageReceived;
        public bool IsConnected => _client?.IsConnected ?? false;


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
            Debug.WriteLine($"Message received on topic {arg.ApplicationMessage.Topic}: {message}");
            Debug.WriteLine($"Length out: {message.Length}");

            // Parse the message payload as JSON
            var jsonMessage = JsonConvert.DeserializeObject<JObject>(message);
            string rfid = jsonMessage["RFID"]?.ToString();
            string density = jsonMessage["Density"]?.ToString();
            string weight = jsonMessage["Weight"]?.ToString();
            string macAddress = jsonMessage["MacAddress"]?.ToString();
            string Mode = jsonMessage["Mode"]?.ToString();

            // Check topic and process corresponding message
            switch (arg.ApplicationMessage.Topic)
            {
                case var topic when topic.EndsWith("/sendRFID"):
                    HandleSendRFID(arg.ApplicationMessage.Topic, rfid);
                    break;

                case var topic when topic.EndsWith("/info"):
                    HandleInfoTopic(rfid, weight, density);
                    break;

                case var topic when topic.EndsWith("/checkmode"):
                    HandleCheckMode(arg.ApplicationMessage.Topic, Mode);
                    break;

                default:
                    Debug.WriteLine("Unexpected topic received");
                    break;
            }
            /*
        case "Can_tieu_ly":
            if (!string.IsNullOrEmpty(rfid) && !string.IsNullOrEmpty(density))
            {
                MessageReceived?.Invoke(this, $"Can_tieu_ly:{rfid}:{density}");
            }
            break;

        case "Can_ta":
            if (!string.IsNullOrEmpty(rfid) && !string.IsNullOrEmpty(weight))
            {
                MessageReceived?.Invoke(this, $"Can_ta:{rfid}:{weight}");
            }
            break;

        case "Canta_info":
            if (!string.IsNullOrEmpty(macAddress) && !string.IsNullOrEmpty(Mode))
            {
                MessageReceived?.Invoke(this, $"CantaInfo:{macAddress}:{Mode}");
            }
            break;

        case "Cantieuly_info":
            if (!string.IsNullOrEmpty(macAddress) && !string.IsNullOrEmpty(Mode))
            {
                MessageReceived?.Invoke(this, $"CantieuLyInfo:{macAddress}:{Mode}");
            }
            break;
            */
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

        private void HandleInfoTopic(string rfid, string weight, string density)
        {
            if (!string.IsNullOrEmpty(rfid))
            {
                if (!string.IsNullOrEmpty(weight))
                {
                    MessageReceived?.Invoke(this, $"info-{rfid}-{weight}-Weight");
                }
                else if (!string.IsNullOrEmpty(density))
                {
                    MessageReceived?.Invoke(this, $"info-{rfid}-{density}-Density");
                }
                else
                {
                    Debug.WriteLine("Info message received but no Weight or Density found");
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
                catch (MqttConnectingFailedException ex)
                {
                    Debug.WriteLine("Kết nối đến máy chủ MQTT thất bại: " + ex.Message);
                    throw new Exception("Kết nối đến máy chủ MQTT thất bại. Vui lòng đảm bảo rằng máy chủ đã được bật.", ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Đã xảy ra lỗi khi kết nối đến máy chủ MQTT: " + ex.Message);
                    throw new Exception("Đã xảy ra lỗi khi kết nối đến máy chủ MQTT. Vui lòng đảm bảo rằng máy chủ đã được bật.", ex);
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
    }
}
