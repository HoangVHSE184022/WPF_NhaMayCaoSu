﻿using WPF_NhaMayCaoSu.Repository.Models;
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

        public async Task UpdateSaleAsync(Sale sale)
        {
            await _repo.UpdateSaleAsync(sale);
        }
    }
}