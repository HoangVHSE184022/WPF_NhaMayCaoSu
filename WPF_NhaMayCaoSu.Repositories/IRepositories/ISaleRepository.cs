using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ISaleRepository
    {
        Task CreateSaleAsync(Sale sale);
        Task<IEnumerable<Sale>> GetAllAsync(int pageNumber, int pageSize);
        Task<Sale> GetSaleByIdAsync(Guid saleId);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleId);
        Task<Sale> GetSaleByRFIDCode(string RFIDCode);
        Task<int> GetTotalSalesCountAsync();
        Task<IEnumerable<Sale>> GetAllSaleAsync();
        Task<Sale> GetLatestSaleWithinTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<Sale>> GetSalesWithoutTotalPriceAsync();

    }
}
