using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IRFIDService
    {
        Task CreateRFIDAsync(RFID RFID);
        Task<IEnumerable<RFID>> GetAllRFIDAsync();
        Task<RFID> GetRFIDByIdAsync(Guid RFID);
        Task UpdateRFIDAsync(RFID RFID);
        Task DeleteRFIDAsync(Guid RFID);
    }
}
