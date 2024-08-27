using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class AuthService : IAuthService
    {
        public async Task<Account> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task Register(Account account)
        {
            throw new NotImplementedException();
        }
    }
}
