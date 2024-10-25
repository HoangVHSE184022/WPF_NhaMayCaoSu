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
            if (customer != null)
            {
                customer.Status = 0;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(int pageNumber, int pageSize)
        {
            _context = new();

            return await _context.Customers
                                 .Include(c => c.RFIDs)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Where(c => c.Status == 1)
                                 .ToListAsync();
        }


        public async Task<Customer> GetCustomerById(Guid id)
        {
            _context = new();
            return await _context.Customers.Include(c => c.RFIDs).FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _context = new();
            _context.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetCustomerByRFIDCodeAsync(string rfidCode)
        {
            _context = new();
            return await _context.Customers
                .Include(c => c.RFIDs)
                .FirstOrDefaultAsync(c => c.RFIDs.Any(r => r.RFIDCode == rfidCode && r.Status == 1));
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            _context = new();
            return await _context.Customers
                .Include(c => c.RFIDs)
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            _context = new();
            return await _context.Customers.CountAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsyncNoPagination()
        {
            _context = new();

            return await _context.Customers
                                 .Include(c => c.RFIDs)
                                 .Where(c => c.Status == 1)
                                 .ToListAsync();
        }
    }
}
