using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class PricingService : IPricingService
    {

        private readonly PricingRepository _repo = new();


        public async Task CreatePricingAsync(Pricing pricing)
        {
            await _repo.CreatePricingAsync(pricing);
        }

        public async Task<IEnumerable<Pricing>> GetAllPricingsAsync(int pageNumber = 1, int pageSize = 5)
        {
            return await _repo.GetAllPricingsAsync(pageNumber, pageSize);
        }

        public async Task<Pricing> GetPricingByIdAsync(Guid pricingId)
        {
            return await _repo.GetPricingByIdAsync(pricingId);
        }

        public async Task UpdatePricingAsync(Pricing pricing)
        {
            await _repo.UpdatePricingAsync(pricing);
        }

        public async Task DeletePricingAsync(Guid pricingId)
        {
            await _repo.DeletePricingAsync(pricingId);
        }
    }
}