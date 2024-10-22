using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class PricingRepository : IPricingRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task CreatePricingAsync(Pricing pricing)
        {
            _context = new();
            await _context.AddAsync(pricing);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Pricing>> GetAllPricingsAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Pricings
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Pricing> GetPricingByIdAsync(Guid pricingId)
        {
            _context = new();
            return await _context.Pricings.FirstOrDefaultAsync(x => x.PricingId == pricingId);
        }

        public async Task UpdatePricingAsync(Pricing pricing)
        {
            _context = new();
            _context.Pricings.Update(pricing);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePricingAsync(Guid pricingId)
        {
            Pricing pricing = await _context.Set<Pricing>().FindAsync(pricingId);
            if (pricing != null)
            {
                _context.Set<Pricing>().Update(pricing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
