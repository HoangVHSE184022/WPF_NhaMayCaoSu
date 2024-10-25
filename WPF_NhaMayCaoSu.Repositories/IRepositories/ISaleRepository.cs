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
        Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize);
        Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId);
        Task<int> GetTotalSalesCountAsync();
        Task<IEnumerable<Sale>> GetAllSaleAsync();
        Task<Sale> GetLatestSaleWithinTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<Sale>> GetSalesWithoutTotalPriceAsync();
        Task<int> GetSalesCountByDateAsync(DateTime date);
        Task<int> GetSalesCountByMonthAsync(int year, int month);
        Task<int> GetSalesCountByYearAsync(int year);


    }
}
