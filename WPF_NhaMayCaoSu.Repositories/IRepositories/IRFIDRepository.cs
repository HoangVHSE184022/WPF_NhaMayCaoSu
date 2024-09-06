using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IRFIDRepository
    {
        Task AddRFIDAsync(RFID rfid);
        Task<IEnumerable<RFID>> GetAllRFIDsAsync(int pageNumber, int pageSize);
        Task<RFID> GetRFIDByIdAsync(Guid rfidId);
        Task UpdateRFIDAsync(RFID rfid);
        Task<RFID> GetRFIDByRFIDCodeAsync(string RFIDCode);
        Task DeleteRFIDAsync(Guid rfidId);
        Task<IEnumerable<RFID>> GetRFIDsByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<RFID>> GetRFIDsByStatusAsync(short status);
        Task<IEnumerable<RFID>> GetRFIDsByExpirationDateAsync(DateTime expirationDate);
        Task<int> GetTotalRFIDsCountAsync();
    }
}
