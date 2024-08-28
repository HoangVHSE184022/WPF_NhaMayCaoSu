using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        public async Task CreateSaleAsync(Sale sale)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteSaleAsync(Guid saleId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Sale>> GetSaleByIdAsync(Guid saleId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            throw new NotImplementedException();
        }
    }
}
