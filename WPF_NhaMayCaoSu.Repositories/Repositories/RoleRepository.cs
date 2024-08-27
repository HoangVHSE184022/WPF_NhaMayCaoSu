using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public async Task CreateRoleAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRoleAsync(Role role)
        {
            throw new NotImplementedException();
        }
    }
}
