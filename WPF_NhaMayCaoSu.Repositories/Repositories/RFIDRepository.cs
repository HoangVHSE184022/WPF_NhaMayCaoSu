using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class RFIDRepository : IRFIDRepository
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

        public async Task<RFID> GetRFIDAsync(Guid RFID)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRFIDAsync(RFID RFID)
        {
            throw new NotImplementedException();
        }
    }
}
