using WPF_NhaMayCaoSu.Repository.IRepositories;
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

        public async Task<Customer?> GetCustomerByRFIDCodeAsync(string rfidCode)
        {
            RFIDService rFIDService = new();
            RFID rFID = await rFIDService.GetRFIDByRFIDCodeAsync(rfidCode);
            return await _repository.GetCustomerByRFIDCodeAsync(rfidCode);
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _repository.GetTotalCustomersCountAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersNoPagination()
        {
            return await _repository.GetAllAsyncNoPagination();
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            return await _repository.GetCustomerByPhoneAsync(phone);

        }

        public async Task<IEnumerable<string>> GetAllCustomerNamesAsync()
        {
            return await _repository.GetAllCustomerNamesAsync();
        }
    }
}
