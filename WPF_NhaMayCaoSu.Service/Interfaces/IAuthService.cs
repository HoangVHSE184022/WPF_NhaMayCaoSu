using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IAuthService
    {
        Task<Account> Login(string username, string password);
        Task Register(Account account);
    }
}
