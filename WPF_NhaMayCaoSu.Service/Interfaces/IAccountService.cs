using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IAccountService
    {
        Task CreateAccountAsync(Account account);
        Task<IEnumerable<Account>> GetAllAccountsAsync(int pageNumber, int pageSize);
        Task<Account> GetAccountByIdAsync(Guid accountId);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Guid accountId);
        Task<Account> LoginAsync(string username, string password);
        Task RegisterAsync(Account account);
        Task<Account> GetAccountByUsernameAsync(string username);
        Task<int> GetTotalAccountsCountAsync();
    }
}
