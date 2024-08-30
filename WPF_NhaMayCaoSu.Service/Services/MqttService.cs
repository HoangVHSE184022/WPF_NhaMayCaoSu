using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Service.Interfaces;

public class MqttService : IMqttService
{
    private readonly MqttServer _mqttServer;
    private readonly ConcurrentDictionary<string, string> _connectedClients;
    public Action ClientsChanged { get; set; }

    public MqttService()
    {
        _connectedClients = new ConcurrentDictionary<string, string>();
        // Configure the MQTT server options for non-TLS
        MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
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
                _connectedClients[e.ClientId] = e.Endpoint; // Store the IP address of the client

                // Notify UI that the clients have changed
                ClientsChanged?.Invoke();

                Console.WriteLine($"Client connected: {e.ClientId}, IP: {e.Endpoint}");
            }
            return Task.CompletedTask;
        };

        _mqttServer.ClientDisconnectedAsync += e =>
        {
            if (_connectedClients != null && !string.IsNullOrEmpty(e.ClientId))
            {
                _connectedClients.TryRemove(e.ClientId, out _);

                // Notify UI that the clients have changed
                ClientsChanged?.Invoke();

                Console.WriteLine($"Client disconnected: {e.ClientId}");
            }
            return Task.CompletedTask;
        };
    }

    public async Task StartBrokerAsync()
    {
        try
        {
            await _mqttServer.StartAsync();
            Console.WriteLine("MQTT broker started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting MQTT broker: {ex.Message}");
        }
    }

    public async Task StopBrokerAsync()
    {
        try
        {
            await _mqttServer.StopAsync();
            Console.WriteLine("MQTT broker stopped.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping MQTT broker: {ex.Message}");
        }
    }

    public async Task RestartBrokerAsync()
    {
        try
        {
            await StopBrokerAsync();
            await StartBrokerAsync();
            Console.WriteLine("MQTT broker restarted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error restarting MQTT broker: {ex.Message}");
        }
    }

    // Get device info that connect to mqtt server
    public IReadOnlyDictionary<string, string> GetConnectedClients()
    {
        return _connectedClients;
    }
}
