using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IConfigService
    {
        Task CreateCameraAsync(Config camera);
        Task<Config> GetCameraAsync();
        Task UpdateCameraAsync(Config camera);
        Task DeleteCameraAsync(Guid id);
        Task<Config> GetCameraByIdAsync(Guid cameraId);
        Task<Config> GetNewestCameraAsync();
    }
}
