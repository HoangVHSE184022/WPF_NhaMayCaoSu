using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task AddCustomer(Customer customer)
        {
            _context = new();
            await _context.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomer(Guid id)
        {
            _context = new();
            Customer customer = await GetCustomerById(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Customers
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Customer> GetCustomerById(Guid id)
        {
            _context = new();
            return await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _context = new();
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
