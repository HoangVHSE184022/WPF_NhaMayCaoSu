using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

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

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
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
            await _accountRepository.Register(account);
        }
    }
}
