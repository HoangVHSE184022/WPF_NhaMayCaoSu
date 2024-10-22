using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IPricingRepository
    {
        Task CreatePricingAsync(Pricing pricing);
        Task<IEnumerable<Pricing>> GetAllPricingsAsync(int pageNumber, int pageSize);
        Task<Pricing> GetPricingByIdAsync(Guid pricingId);
        Task UpdatePricingAsync(Pricing pricing);
        Task DeletePricingAsync(Guid pricingId);
    }
}
