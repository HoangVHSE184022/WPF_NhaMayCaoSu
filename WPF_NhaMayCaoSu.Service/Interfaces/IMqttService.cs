namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttService
    {
        Task StartBrokerAsync();
        Task StopBrokerAsync();
        Task RestartBrokerAsync();
    }
}
