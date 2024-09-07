using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ICustomerRepository
    {
        Task AddCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task DeleteCustomer(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync(int pageNumber, int pageSize);
        Task<Customer> GetCustomerById(Guid id);
        Task<Customer?> GetCustomerByRFIDCodeAsync(string rfidCode);
        Task<int> GetTotalCustomersCountAsync();
    }
}
