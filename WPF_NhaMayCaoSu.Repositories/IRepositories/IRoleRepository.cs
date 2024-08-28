using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IRoleRepository
    {
        Task CreateRoleAsync(Role role);
        Task<IEnumerable<Role>> GetAllRolesAsync(int pageNumber, int pageSize);
        Task<Role> GetRoleByIdAsync(Guid roleId);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(Guid roleId);
    }
}
