using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class AccountService : IAccountService
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

        public async Task UpdateAccountAsync(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
