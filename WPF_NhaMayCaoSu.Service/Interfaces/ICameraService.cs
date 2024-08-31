using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ICameraService
    {
        Task CreateCameraAsync(Camera camera);
        Task<Camera> GetCameraAsync();
        Task UpdateCameraAsync(Camera camera);
        Task DeleteCameraAsync(Guid id);
        Task<Camera> GetCameraByIdAsync(Guid cameraId);
        Task<Camera> GetNewestCameraAsync();
    }
}
