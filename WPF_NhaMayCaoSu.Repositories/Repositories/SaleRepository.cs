using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private CaoSuWpfDbContext _context;
        public async Task CreateSaleAsync(Sale sale)
        {
            _context = new();
            await _context.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSaleAsync(Guid saleId)
        {
            _context = new();
            Sale sale = await _context.Sales.FirstOrDefaultAsync(x => x.SaleId == saleId);
            if (sale != null)
            {
                sale.Status = 0;
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Sale>> GetAllAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Sales
                                 .OrderByDescending(s => s.LastEditedTime)
                                 .ThenBy(s => s.CustomerName)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Where(c => c.Status == 1)
                                 .ToListAsync();
        }


        public async Task<Sale> GetSaleByIdAsync(Guid saleId)
        {
            _context = new();
            return await _context.Sales.Include("Customer").FirstOrDefaultAsync(x => x.SaleId == saleId);
        }

        public async Task<Sale> GetSaleByRFIDCode(string RFIDCode)
        {
            _context = new();
            return await _context.Sales
                                 .Where(c => c.Status == 1)
                                 .Include(s => s.RFID)
                                 .FirstOrDefaultAsync(x => x.RFIDCode == RFIDCode);
        }

        public async Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId)
        {
            _context = new();

            return await _context.Sales
                                 .Include(s => s.RFID)
                                 .Where(s => s.RFID.CustomerId == customerId && s.Status == 1)
                                 .ToListAsync();
        }

        public async Task<Sale> GetSaleByRFIDCodeWithoutDensity(string RFIDCode)
        {
            _context = new();
            return await _context.Sales
                                 .Where(c => c.Status == 1)
                                 .Include(s => s.RFID)
                                 .FirstOrDefaultAsync(x => x.RFIDCode == RFIDCode && x.ProductDensity == 0);
        }


        public async Task UpdateSaleAsync(Sale sale)
        {
            _context = new();
            _context.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalSalesCountAsync()
        {
            _context = new();
            return await _context.Sales.CountAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllSaleAsync()
        {
            _context = new();
            return await _context.Sales.Where(s => s.Status == 1)
                                 .ToListAsync();
        }

        public async Task<Sale> GetLatestSaleWithinTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            _context = new();
            return await _context.Sales
                                 .Where(s => s.LastEditedTime >= startTime && s.LastEditedTime <= endTime)
                                 .OrderByDescending(s => s.LastEditedTime)
                                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Sale>> GetSalesWithoutTotalPriceAsync()
        {
            return await _context.Sales
                .Where(s => s.ProductDensity != null
                         && s.ProductWeight != null
                         && s.TareWeight != null
                         && s.SalePrice != null
                         && s.BonusPrice != null
                         && s.TotalPrice == null)
                .ToListAsync();
        }

        public async Task<int> GetSalesCountByDateAsync(DateTime date)
        {
            _context = new();
            return await _context.Sales
                                 .Where(s => s.LastEditedTime.Value.Date == date.Date && s.Status == 1)
                                 .CountAsync();
        }

        public async Task<int> GetSalesCountByMonthAsync(int year, int month)
        {
            _context = new();
            return await _context.Sales
                                 .Where(s => s.LastEditedTime.Value.Year == year && s.LastEditedTime.Value.Month == month && s.Status == 1)
                                 .CountAsync();
        }

        public async Task<int> GetSalesCountByYearAsync(int year)
        {
            _context = new();
            return await _context.Sales
                                 .Where(s => s.LastEditedTime.Value.Year == year && s.Status == 1)
                                 .CountAsync();
        }

    }
}
