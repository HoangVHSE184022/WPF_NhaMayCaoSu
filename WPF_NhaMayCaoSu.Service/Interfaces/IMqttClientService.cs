

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttClientService
    {
        Task ConnectAsync();
        Task SubscribeAsync(string topic);
        Task PublishAsync(string topic, string payload);
        Task CloseConnectionAsync();
    }
}
