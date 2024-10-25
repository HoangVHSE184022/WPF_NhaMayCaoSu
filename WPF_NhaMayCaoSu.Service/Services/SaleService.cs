using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class SaleService : ISaleService
    {
        private readonly SaleRepository _repo = new();
        public async Task CreateSaleAsync(Sale sale)
        {
            await _repo.CreateSaleAsync(sale);
        }

        public async Task DeleteSaleAsync(Guid saleId)
        {
            await _repo.DeleteSaleAsync(saleId);
        }

        public async Task<IEnumerable<Sale>> GetAllSaleAsync(int PageNumber = 1, int pageSize = 5)
        {
            return await _repo.GetAllAsync(PageNumber, pageSize);
        }

        public async Task<Sale> GetSaleByIdAsync(Guid saleId)
        {
            return await _repo.GetSaleByIdAsync(saleId);
        }

        public async Task<Sale> GetSaleByRfidAsync(string rfid)
        {
            return await _repo.GetSaleByRFIDCode(rfid);
        }

        public async Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId)
        {
            return await _repo.GetSalesByCustomerIdAsync(customerId);
        }

        public async Task<Sale> GetSaleByRFIDCodeWithoutDensity(string rfid)
        {
            return await _repo.GetSaleByRFIDCodeWithoutDensity(rfid);
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            await _repo.UpdateSaleAsync(sale);
        }

        public async Task<int> GetTotalSalesCountAsync()
        {
            return await _repo.GetTotalSalesCountAsync();
        }
        public async Task<IEnumerable<Sale>> GetAllSaleAsync()
        {
            return await _repo.GetAllSaleAsync();
        }

        public async Task<Sale> GetLatestSaleWithinTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            return await _repo.GetLatestSaleWithinTimeRangeAsync(startTime, endTime);
        }

        public async Task<IEnumerable<Sale>> GetSalesWithoutTotalPriceAsync()
        {
            return await _repo.GetSalesWithoutTotalPriceAsync();
        }

        public async Task<int> GetSalesCountByDateAsync(DateTime date)
        {
            return await _repo.GetSalesCountByDateAsync(date);
        }

        public async Task<int> GetSalesCountByMonthAsync(int year, int month)
        {
            return await _repo.GetSalesCountByMonthAsync(year, month);
        }

        public async Task<int> GetSalesCountByYearAsync(int year)
        {
            return await _repo.GetSalesCountByYearAsync(year);
        }
    }
}
