using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class RFIDService : IRFIDService
    {
        public async Task CreateRFIDAsync(RFID RFID)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRFIDAsync(Guid RFID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RFID>> GetAllRFIDAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<RFID> GetRFIDByIdAsync(Guid RFID)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRFIDAsync(RFID RFID)
        {
            throw new NotImplementedException();
        }
    }
}
