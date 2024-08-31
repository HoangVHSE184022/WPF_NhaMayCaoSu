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

        public async Task<IEnumerable<Customer>> GetAllCustomers(int pageNumber = 1, int pageSize = 5)
        {
            return await _repository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<Customer> GetCustomerById(Guid id)
        {
            return await _repository.GetCustomerById(id);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            await _repository.UpdateCustomer(customer);
        }

        public async Task<int> CountCustomerRFIDs(Guid customerId)
        {
            var customer = await _repository.GetCustomerById(customerId);
            return customer?.RFIDs?.Count ?? 0;
        }
    }
}
