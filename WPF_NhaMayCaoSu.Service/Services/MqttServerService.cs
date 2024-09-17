using MQTTnet;
using MQTTnet.Server;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

public class MqttServerService : IMqttServerService
{
    private static readonly Lazy<MqttServerService> _instance = new(() => new MqttServerService());
    public static MqttServerService Instance => _instance.Value;
    private readonly MQTTnet.Server.MqttServer _mqttServer;
    private readonly ConcurrentDictionary<string, string> _connectedClients;
    private readonly List<BoardModelView> _connectedBoard;
    private readonly IBoardService _boardService;
    public event EventHandler ClientsChanged;
    public event EventHandler<int> DeviceCountChanged;
    public event EventHandler BrokerStatusChanged;
    public event EventHandler BoardReceived;
    private int _deviceCount;
    public static bool IsBrokerRunning { get; set; } = false;

    public MqttServerService()
    {
        _connectedClients = new ConcurrentDictionary<string, string>();
        _boardService = new BoardService();
        _connectedBoard = new List<BoardModelView>();
        _deviceCount = 0;

        MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Any)
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .WithConnectionBacklog(100)
            .WithMaxPendingMessagesPerClient(1000);

        _mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

        _mqttServer.ValidatingConnectionAsync += e =>
        {
            if (e.UserName != "admin" || e.Password != "admin")
            {
                e.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
            }
            else
            {
                e.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
            }

            return Task.CompletedTask;
        };

        _mqttServer.ClientConnectedAsync += e =>
        {
            if (_connectedClients != null && !string.IsNullOrEmpty(e.ClientId))
            {
                string clientIp = e.Endpoint.ToString().Split(':')[0];
                _connectedClients[e.ClientId] = clientIp;
                _deviceCount++;
                ClientsChanged?.Invoke(this, EventArgs.Empty);
                DeviceCountChanged?.Invoke(this, _deviceCount);
            }
            return Task.CompletedTask;
        };

        _mqttServer.ClientDisconnectedAsync += e =>
        {
            if (_connectedClients != null && !string.IsNullOrEmpty(e.ClientId))
            {
                _connectedClients.TryRemove(e.ClientId, out _);
                _deviceCount--;
                ClientsChanged?.Invoke(this, EventArgs.Empty);
                DeviceCountChanged?.Invoke(this, _deviceCount);
            }
            return Task.CompletedTask;
        };

        _mqttServer.InterceptingPublishAsync += async e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            if (topic == "connect")
            {

                try
                {
                    // Parse JSON to a dictionary
                    var message = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payload);

                    if (message == null || !message.ContainsKey("MacAddress") || !message.ContainsKey("Mode"))
                    {
                        Debug.WriteLine("Invalid payload structure.");
                        return;
                    }

                    string macAddress = message["MacAddress"].GetString();
                    int mode = message["Mode"].GetInt32();

                    var board = _connectedBoard.FirstOrDefault(b => b.BoardMacAddress == macAddress);
                    Board boardDB = await _boardService.GetBoardByMacAddressAsync(macAddress);

                    if (boardDB != null && boardDB.BoardMode != mode)
                    {
                        boardDB.BoardMode = mode;
                        _boardService.UpdateBoardAsync(boardDB);
                    }

                    if (board == null)
                    {
                        board = new BoardModelView
                        {
                            BoardId = Guid.NewGuid(),
                            BoardName = e.ClientId,
                            BoardIp = _connectedClients.ContainsKey(e.ClientId) ? _connectedClients[e.ClientId] : "Unknown",
                            BoardMacAddress = macAddress,
                            BoardMode = mode
                        };

                        _connectedBoard.Add(board);
                    }
                    else
                    {
                        board.BoardMacAddress = macAddress;
                        board.BoardMode = mode;
                        board.BoardIp = _connectedClients.ContainsKey(e.ClientId) ? _connectedClients[e.ClientId] : "Unknown";
                    }

                    BoardReceived?.Invoke(this, EventArgs.Empty);
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"Error parsing JSON payload: {ex.Message}");
                }
            }

            await Task.CompletedTask;
        };
    }

    public async Task StartBrokerAsync()
    {
        try
        {
            await _mqttServer.StartAsync();
            IsBrokerRunning = true;
            OnBrokerStatusChanged();
            Debug.WriteLine("MQTT broker started.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting MQTT broker: {ex.Message}");
        }
    }

    public async Task StopBrokerAsync()
    {
        try
        {
            await _mqttServer.StopAsync();
            IsBrokerRunning = false;
            OnBrokerStatusChanged();
            Debug.WriteLine("MQTT broker stopped.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error stopping MQTT broker: {ex.Message}");
        }
    }

    protected virtual void OnBrokerStatusChanged()
    {
        BrokerStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task RestartBrokerAsync()
    {
        try
        {
            await StopBrokerAsync();
            await StartBrokerAsync();
            IsBrokerRunning = true;
            OnBrokerStatusChanged();
            Debug.WriteLine("MQTT broker restarted.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error restarting MQTT broker: {ex.Message}");
        }
    }

    public int GetDeviceCount()
    {
        return _deviceCount;
    }

    public IReadOnlyDictionary<string, string> GetConnectedClients()
    {
        return _connectedClients;
    }

    public List<BoardModelView> GetConnectedBoard()
    {
        return _connectedBoard;
    }
}
