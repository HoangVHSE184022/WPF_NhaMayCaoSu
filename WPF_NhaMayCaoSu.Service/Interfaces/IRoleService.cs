using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IRoleService
    {
        Task CreateRoleAsync(Role role);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(Guid roleId);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(Guid roleId);
    }
}
