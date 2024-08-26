using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(Guid accountId);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Guid accountId);
        Task<Account> Login(string username, string password);
        Task Register(Account account);

    }
}
