﻿

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttServerService
    {
        Task StartBrokerAsync();
        Task StopBrokerAsync();
        Task RestartBrokerAsync();
        IReadOnlyDictionary<string, string> GetConnectedClients();

        event EventHandler ClientsChanged;
    }
}