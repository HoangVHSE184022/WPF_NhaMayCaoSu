using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using WPF_NhaMayCaoSu.Service.Interfaces;

public class MqttService : IMqttService
{
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;

    public MqttService()
    {
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
            .WithClientId("WPF_Client")
            .WithTcpServer("broker.hivemq.com", 1883) 
            .WithCleanSession()
            .Build();

        // Register event handlers
        _client.ConnectedAsync += OnConnectedAsync;
        _client.DisconnectedAsync += OnDisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
    }

    private Task OnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        Console.WriteLine("Connected to MQTT broker.");
        return Task.CompletedTask;
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        Console.WriteLine("Disconnected from MQTT broker.");
        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var message = System.Text.Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
        Console.WriteLine($"Message received on topic {arg.ApplicationMessage.Topic}: {message}");
        return Task.CompletedTask;
    }

    public async Task ConnectAsync()
    {
        await _client.ConnectAsync(_options);
    }

    public async Task SubscribeAsync(string topic)
    {
        await _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();

        await _client.PublishAsync(message);
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await _client.ConnectAsync(_options);
            return _client.IsConnected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MQTT broker: {ex.Message}");
            return false;
        }
    }

}
