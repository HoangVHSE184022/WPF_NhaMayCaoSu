using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class RFIDRepository : IRFIDRepository
    {
        private CaoSuWpfDbContext? _context;

        public async Task AddRFIDAsync(RFID rfid)
        {
            _context = new CaoSuWpfDbContext();
            await _context.RFIDs.AddAsync(rfid);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RFID>> GetAllRFIDsAsync(int pageNumber, int pageSize)
        {
            _context = new CaoSuWpfDbContext();

            return await _context.RFIDs
                                 .Include("Customer")
                                 .Where(r => r.Status == 1)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<RFID> GetRFIDByIdAsync(Guid rfidId)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.RFIDs.FirstOrDefaultAsync(r => r.RFID_Id == rfidId);
        }

        public async Task<RFID> GetRFIDByRFIDCodeAsync(string RFIDCode)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.RFIDs.FirstOrDefaultAsync(r => r.RFIDCode == RFIDCode);
        }

        public async Task UpdateRFIDAsync(RFID rfid)
        {
            _context = new CaoSuWpfDbContext();
            _context.RFIDs.Update(rfid);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRFIDAsync(Guid rfidId)
        {
            _context = new CaoSuWpfDbContext();
            RFID rfid = await _context.RFIDs.FindAsync(rfidId);
            if (rfid != null)
            {
                rfid.Status = 0;
                _context.RFIDs.Update(rfid);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByCustomerIdAsync(Guid customerId)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.RFIDs
                                 .Where(r => r.CustomerId == customerId && r.Status == 1)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByStatusAsync(short status)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.RFIDs
                                 .Where(r => r.Status == status)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<RFID>> GetRFIDsByExpirationDateAsync(DateTime expirationDate)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.RFIDs
                                 .Where(r => r.ExpirationDate.Date == expirationDate.Date && r.Status == 1)
                                 .ToListAsync();
        }
    }
}
