﻿using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ISaleService
    {
        Task CreateSaleAsync(Sale sale);
        Task<IEnumerable<Sale>> GetAllSaleAsync(int pageNumber, int pageSize);
        Task<Sale> GetSaleByIdAsync(Guid saleId);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleId);
        Task<Sale> GetSaleByRfidAsync(string rfid);
        Task<Sale> GetSaleByRFIDCodeWithoutDensity(string rfid);
        Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId, int pageNumber, int pageSize);
        Task<IEnumerable<Sale>> GetSalesByCustomerIdAsync(Guid customerId);
        Task<int> GetTotalSalesCountAsync();
        Task<IEnumerable<Sale>> GetAllSaleAsync();
        Task<Sale> GetLatestSaleWithinTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<IEnumerable<Sale>> GetSalesWithoutTotalPriceAsync();
    }
}
