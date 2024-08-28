using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _repository = new();
        public async Task CreateCustomer(Customer customer)
        {
            await _repository.AddCustomer(customer);
        }

        public async Task DeleteCustomer(Guid id)
        {
            await _repository.DeleteCustomer(id);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await _repository.GetAll();
        }

        public async Task<Customer> GetCustomerById(Guid id)
        {
            return await _repository.GetCustomerById(id);   
        }

        public async Task UpdateCustomer(Customer customer)
        {
            await _repository.UpdateCustomer(customer);
        }
    }
}
