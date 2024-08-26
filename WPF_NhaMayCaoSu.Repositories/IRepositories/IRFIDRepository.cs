using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IRFIDRepository
    {
        Task CreateRFIDAsync(RFID RFID);
        Task <IEnumerable<RFID>> GetAllRFIDAsync();
        Task<RFID> GetRFIDAsync(Guid RFID);
        Task UpdateRFIDAsync(RFID RFID);
        Task DeleteRFIDAsync(Guid RFID);


    }
}
