using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IPricingService
    {
        Task CreatePricingAsync(Pricing pricing);
        Task<IEnumerable<Pricing>> GetAllPricingsAsync(int pageNumber, int pageSize);
        Task<Pricing> GetPricingByIdAsync(Guid pricingId);
        Task UpdatePricingAsync(Pricing pricing);
        Task DeletePricingAsync(Guid pricingId);
    }
}
