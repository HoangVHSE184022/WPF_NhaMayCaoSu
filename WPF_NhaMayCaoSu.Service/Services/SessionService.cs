using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class SessionService : ISessionService
    {
        private readonly List<Sale> _sessionSaleList;

        public SessionService()
        {
            _sessionSaleList = new List<Sale>();
        }

        public void AddToSalelist(Sale sale)
        {
            _sessionSaleList.Add(sale);
        }

        public List<Sale> GetAllSales()
        {
            return _sessionSaleList.OrderBy(sale => sale.CreatedDate).ToList();
        }

        public List<Sale> GetSaleByRFID(string RfID)
        {
            return _sessionSaleList.Where(sale => sale.RFIDCode == RfID).OrderBy(sale => sale.CreatedDate).ToList();
        }
    }
}
