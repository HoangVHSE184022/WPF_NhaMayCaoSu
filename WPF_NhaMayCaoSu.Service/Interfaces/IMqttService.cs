namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttService
    {
        Task ConnectAsync();
        Task SubscribeAsync(string topic);
        Task PublishAsync(string topic, string payload);
    }
}
