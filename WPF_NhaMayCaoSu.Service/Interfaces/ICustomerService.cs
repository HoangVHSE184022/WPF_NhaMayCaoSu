using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ICustomerService
    {
        Task CreateCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task DeleteCustomer(Guid id);
        Task<IEnumerable<Customer>> GetAllCustomers(int pageNumber, int pageSize);
        Task<Customer> GetCustomerById(Guid id);
    }
}
