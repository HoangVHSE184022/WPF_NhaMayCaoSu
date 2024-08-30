using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttService
    {
        Task StartBrokerAsync();
        Task StopBrokerAsync();
        Task RestartBrokerAsync();
        IReadOnlyDictionary<string, string> GetConnectedClients();

        event EventHandler ClientsChanged;
    }
}
