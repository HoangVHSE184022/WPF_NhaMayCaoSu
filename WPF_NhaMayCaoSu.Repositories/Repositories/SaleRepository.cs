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
            _context.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Sales
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Sale> GetSaleByIdAsync(Guid saleId)
        {
            _context = new();
            return await _context.Sales.FirstOrDefaultAsync(x => x.SaleId == saleId);
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _context = new();
            _context.Update(sale);
            await _context.SaveChangesAsync();
        }
    }
}
