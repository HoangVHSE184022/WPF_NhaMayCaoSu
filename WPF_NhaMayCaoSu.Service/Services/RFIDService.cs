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
            await _repo.AddRFID(rfid);
        }
        public async Task<IEnumerable<RFID>> GetAllRFIDsAsync(int pageNumber, int pageSize)
        {
            return await _repo.GetAllRFIDs(pageNumber, pageSize);
        }

        public async Task<RFID> GetRFIDByIdAsync(Guid rfidId)
        {
            return await _repo.GetRFIDById(rfidId);
        }

        public async Task UpdateRFIDAsync(RFID rfid)
        {
            await _repo.UpdateRFID(rfid);
        }

        public async Task DeleteRFIDAsync(Guid rfidId)
        {
            await _repo.DeleteRFID(rfidId);
        }
        public async Task<IEnumerable<RFID>> GetRFIDsByCustomerIdAsync(Guid customerId)
        {
            return await _repo.GetRFIDsByCustomerId(customerId);
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByStatusAsync(short status)
        {
            return await _repo.GetRFIDsByStatus(status);
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByExpirationDateAsync(DateTime expirationDate)
        {
            return await _repo.GetRFIDsByExpirationDate(expirationDate);
        }
    }
}
