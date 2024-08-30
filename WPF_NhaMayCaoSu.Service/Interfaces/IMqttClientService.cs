

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttClientService
    {
        event EventHandler<string> MessageReceived;
        Task ConnectAsync();
        Task SubscribeAsync(string topic);
        Task PublishAsync(string topic, string payload);
        Task CloseConnectionAsync();
    }
}
