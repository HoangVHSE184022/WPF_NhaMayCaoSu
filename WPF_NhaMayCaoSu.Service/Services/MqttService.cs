using MQTTnet;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Service.Interfaces;

public class MqttService : IMqttService
{
    private readonly MqttServer _mqttServer;

    public MqttService()
    {
        // Configure the MQTT server options
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1884)  
            .WithConnectionBacklog(100)     
            .WithMaxPendingMessagesPerClient(1000); 

        // Create the MQTT server
        _mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());

        // Set up event handler for connection validation with credentials
        _mqttServer.ValidatingConnectionAsync += e =>
        {
            if (e.ClientId != "e_scale" ||
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
            Console.WriteLine($"Client connected: {e.ClientId}");
            return Task.CompletedTask;
        };

        _mqttServer.ClientDisconnectedAsync += e =>
        {
            Console.WriteLine($"Client disconnected: {e.ClientId}");
            return Task.CompletedTask;
        };
    }

    public async Task StartBrokerAsync()
    {
        // Start the MQTT server
        await _mqttServer.StartAsync();
        Console.WriteLine("MQTT broker started.");
    }

    public async Task StopBrokerAsync()
    {
        // Stop the MQTT server
        await _mqttServer.StopAsync();
        Console.WriteLine("MQTT broker stopped.");
    }

    public async Task RestartBrokerAsync()
    {
        await StopBrokerAsync();
        await StartBrokerAsync();
        Console.WriteLine("MQTT broker restarted.");
    }
}
