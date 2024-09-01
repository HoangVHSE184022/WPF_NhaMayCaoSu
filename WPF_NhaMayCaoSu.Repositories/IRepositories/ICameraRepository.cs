using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ICameraRepository
    {
        Task AddCameraAsync(Camera camera);
        Task<Camera> GetCameraAsync();
        Task UpdateCameraAsync(Camera camera);
        Task DeleteCameraAsync(Guid id);
        Task<Camera> GetCameraByIdAsync(Guid cameraId);
        Task<Camera> GetNewestCameraAsync();
    }
}