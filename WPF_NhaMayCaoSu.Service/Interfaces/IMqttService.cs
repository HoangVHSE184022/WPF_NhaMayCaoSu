using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IMqttService
    {
        Task ConnectAsync();
        Task SubscribeAsync(string topic);
        Task PublishAsync(string topic, string payload);
        Task<bool> TestConnectionAsync();
    }
}
