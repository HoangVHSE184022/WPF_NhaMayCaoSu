using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class RFIDService : IRFIDService
    {
        private readonly RFIDRepository _repo = new();

        public async Task AddRFIDAsync(RFID rfid)
        {
            await _repo.AddRFIDAsync(rfid);
        }
        public async Task<IEnumerable<RFID>> GetAllRFIDsAsync(int pageNumber, int pageSize)
        {
            return await _repo.GetAllRFIDsAsync(pageNumber, pageSize);
        }

        public async Task<RFID> GetRFIDByIdAsync(Guid rfidId)
        {
            return await _repo.GetRFIDByIdAsync(rfidId);
        }

        public async Task<RFID> GetRFIDByRFIDCodeAsync(string RFIDCode)
        {
            return await _repo.GetRFIDByRFIDCodeAsync(RFIDCode);
        }

        public async Task UpdateRFIDAsync(RFID rfid)
        {
            await _repo.UpdateRFIDAsync(rfid);
        }

        public async Task DeleteRFIDAsync(Guid rfidId)
        {
            await _repo.DeleteRFIDAsync(rfidId);
        }
        public async Task<IEnumerable<RFID>> GetRFIDsByCustomerIdAsync(Guid customerId)
        {
            return await _repo.GetRFIDsByCustomerIdAsync(customerId);
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByStatusAsync(short status)
        {
            return await _repo.GetRFIDsByStatusAsync(status);
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByExpirationDateAsync(DateTime expirationDate)
        {
            return await _repo.GetRFIDsByExpirationDateAsync(expirationDate);
        }

        public async Task<int> GetTotalRFIDsCountAsync()
        {
            return await _repo.GetTotalRFIDsCountAsync();
        }
        public async Task<IEnumerable<RFID>> GetAllRFIDsAsync()
        {
            return await _repo.GetAllRFIDsAsync();
        }
    }
}
