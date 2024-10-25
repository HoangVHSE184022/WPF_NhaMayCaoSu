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
        Task<int> CountCustomerRFIDs(Guid customerId);
        Task<Customer?> GetCustomerByRFIDCodeAsync(string rfidCode);
        Task<int> GetTotalCustomersCountAsync();
        Task<IEnumerable<Customer>> GetAllCustomersNoPagination();
        Task<Customer> GetCustomerByPhoneAsync(string phone);
    }
}
