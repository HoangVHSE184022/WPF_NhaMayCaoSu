using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ISessionService
    {
        List<Sale> GetAllSale();
        List<Sale> GetSaleByRFID(long RfID);
        void AddToSalelist(Sale sale);

    }
}
