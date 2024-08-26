using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ISaleService
    {
        Task CreateSaleAsync(Sale sale);
        Task<IEnumerable<Sale>> GetAllSaleAsync();
        Task<Sale> GetSaleByIdAsync(Guid saleId);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Guid saleId);
    }
}
