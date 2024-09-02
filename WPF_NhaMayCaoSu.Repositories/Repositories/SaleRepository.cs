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
                                 .Where(x => x.Status == 1)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
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
            return await _context.Sales.Include("Customer").FirstOrDefaultAsync(x => x.RFIDCode == RFIDCode);
        }

        public async Task<Sale> GetSaleByRFIDCodeWithoutDensity(string RFIDCode)
        {
            _context = new();
            return await _context.Sales.Include("Customer").FirstOrDefaultAsync(x => x.RFIDCode == RFIDCode && x.ProductDensity == null);
        }


        public async Task UpdateSaleAsync(Sale sale)
        {
            _context = new();
            _context.Update(sale);
            await _context.SaveChangesAsync();
        }
    }
}
