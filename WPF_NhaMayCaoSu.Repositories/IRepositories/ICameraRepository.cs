using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ICameraRepository
    {
        Task AddCamera(Camera camera);
        Task<Camera> GetCamera();
        Task UpdateCamera(Camera camera);
        Task DeleteCamera(Guid id);
    }
}