using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ISaleRepository
    {
        Task CreateSaleAsync(Sale sale);
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<IEnumerable<Sale>> GetSaleByIdAsync(Guid saleId);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleId);
    }
}
