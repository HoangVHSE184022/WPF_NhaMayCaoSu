using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class AccountService : IAccountService
    {

        private readonly AccountRepository _accountRepository = new();

        public async Task CreateAccountAsync(Account account)
        {
            await _accountRepository.CreateAccountAsync(account);
        }

        public async Task DeleteAccountAsync(Guid accountId)
        {
            await _accountRepository.DeleteAccountAsync(accountId);
        }

        public async Task<Account> GetAccountByIdAsync(Guid accountId)
        {
            return await _accountRepository.GetAccountByIdAsync(accountId);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync(int pageNumber = 1, int pageSize = 5)
        {
            return await _accountRepository.GetAllAccountsAsync(pageNumber, pageSize);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await _accountRepository.UpdateAccountAsync(account);
        }

        public async Task<Account> LoginAsync(string username, string password)
        {
            return await _accountRepository.Login(username, password);
        }

        public async Task RegisterAsync(Account account)
        {
            account.AccountId = Guid.NewGuid();
            account.CreatedDate = DateTime.UtcNow;
            account.Status = 1;
            await _accountRepository.Register(account);
        }
    }
}
