using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CaoSuWpfDbContext _context;
        public AccountRepository(CaoSuWpfDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Account> GetAccountByIdAsync(Guid accountId)
        {
            return await _context.Accounts
                                 .Include(a => a.Role)
                                 .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync(int pageNumber, int pageSize)
        {
            return await _context.Accounts
                                 .Include(a => a.Role)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }


        public async Task<Account> Login(string username, string password)
        {
            return await _context.Accounts
                               .Include(a => a.Role)
                               .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
        }

        public async Task Register(Account account)
        {
            account.CreatedDate = DateTime.UtcNow;
            account.Status = 1;
            await CreateAccountAsync(account);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
