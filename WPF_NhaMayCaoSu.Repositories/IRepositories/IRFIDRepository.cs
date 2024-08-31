using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IRFIDRepository
    {
        Task AddRFID(RFID rfid);
        Task<IEnumerable<RFID>> GetAllRFIDs(int pageNumber, int pageSize);
        Task<RFID> GetRFIDById(Guid rfidId);
        Task UpdateRFID(RFID rfid);
        Task DeleteRFID(Guid rfidId);
        Task<IEnumerable<RFID>> GetRFIDsByCustomerId(Guid customerId);
        Task<IEnumerable<RFID>> GetRFIDsByStatus(short status);
        Task<IEnumerable<RFID>> GetRFIDsByExpirationDate(DateTime expirationDate);
    }
}
