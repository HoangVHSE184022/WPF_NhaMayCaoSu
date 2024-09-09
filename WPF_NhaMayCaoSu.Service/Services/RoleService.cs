using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class RoleService : IRoleService
    {

        private readonly RoleRepository _repo = new();


        public async Task CreateRoleAsync(Role role)
        {
            await _repo.CreateRoleAsync(role);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(int pageNumber = 1, int pageSize = 5)
        {
            return await _repo.GetAllRolesAsync(pageNumber, pageSize);
        }

        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            return await _repo.GetRoleByIdAsync(roleId);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            await _repo.UpdateRoleAsync(role);
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            await _repo.DeleteRoleAsync(roleId);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _repo.GetRoleByNameAsync(roleName);
        }
    }
}
