using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IRFIDService
    {
        Task AddRFIDAsync(RFID rfid);
        Task<IEnumerable<RFID>> GetAllRFIDsAsync(int pageNumber, int pageSize);
        Task<RFID> GetRFIDByIdAsync(Guid rfidId);
        Task UpdateRFIDAsync(RFID rfid);
        Task DeleteRFIDAsync(Guid rfidId);
        Task<RFID> GetRFIDByRFIDCodeAsync(string RFIDCode);
        Task<IEnumerable<RFID>> GetRFIDsByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<RFID>> GetRFIDsByStatusAsync(short status);
        Task<IEnumerable<RFID>> GetRFIDsByExpirationDateAsync(DateTime expirationDate);
        Task<int> GetTotalRFIDsCountAsync();
    }
}
