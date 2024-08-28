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
    }
}
