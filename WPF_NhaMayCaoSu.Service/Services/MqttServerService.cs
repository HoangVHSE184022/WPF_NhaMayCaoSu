using Azure.Core;
using MQTTnet;
using MQTTnet.Server;
using System.Collections.Concurrent;
using System.Diagnostics;
using WPF_NhaMayCaoSu.Service.Interfaces;

public class MqttServerService : IMqttServerService
{
    private readonly MQTTnet.Server.MqttServer _mqttServer;
    private readonly ConcurrentDictionary<string, string> _connectedClients;
    public event EventHandler ClientsChanged;
    public event EventHandler<int> DeviceCountChanged;
    private int _deviceCount;

    public MqttServerService()
    {
        _connectedClients = new ConcurrentDictionary<string, string>();
        _deviceCount = 0;
        // Configure the MQTT server options for non-TLS
        MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Any)
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883) 
            .WithConnectionBacklog(100)
            .WithMaxPendingMessagesPerClient(1000);

        // Create the MQTT server
        _mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

        // Set up event handler for connection validation with credentials
        _mqttServer.ValidatingConnectionAsync += e =>
        {
            if (
                e.UserName != "admin" ||
                e.Password != "admin")
            {
                e.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
            }
            else
            {
                e.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
            }

            return Task.CompletedTask;
        };

        // Set up event handlers for client connections, disconnections, etc.
        _mqttServer.ClientConnectedAsync += e =>
        {
            if (_connectedClients != null && !string.IsNullOrEmpty(e.ClientId))
            {
                // Extract the IP address from the endpoint
                string clientIp = e.Endpoint.ToString().Split(':')[0];

                _connectedClients[e.ClientId] = clientIp;
                _deviceCount++;

                ClientsChanged?.Invoke(this, EventArgs.Empty); // Trigger the event
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

                ClientsChanged?.Invoke(this, EventArgs.Empty); // Trigger the event
                DeviceCountChanged?.Invoke(this, _deviceCount);
            }
            return Task.CompletedTask;
        };
    }

    public async Task StartBrokerAsync()
    {
        try
        {
            await _mqttServer.StartAsync();
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
            Debug.WriteLine("MQTT broker stopped.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error stopping MQTT broker: {ex.Message}");
        }
    }

    public async Task RestartBrokerAsync()
    {
        try
        {
            await StopBrokerAsync();
            await StartBrokerAsync();
            Debug.WriteLine("MQTT broker restarted.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error restarting MQTT broker: {ex.Message}");
        }
    }

    // Get device info that connect to mqtt server
    public IReadOnlyDictionary<string, string> GetConnectedClients()
    {
        return _connectedClients;
    }
}
