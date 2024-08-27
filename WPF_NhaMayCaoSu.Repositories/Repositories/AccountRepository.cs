using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public async Task CreateAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<Account> GetAccountByIdAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Account> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task Register(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
