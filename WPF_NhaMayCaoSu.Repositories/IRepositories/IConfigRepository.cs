using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IConfigRepository
    {
        Task AddCameraAsync(Config camera);
        Task<Config> GetCameraAsync();
        Task UpdateCameraAsync(Config camera);
        Task DeleteCameraAsync(Guid id);
        Task<Config> GetCameraByIdAsync(Guid cameraId);
        Task<Config> GetNewestCameraAsync();
    }
}