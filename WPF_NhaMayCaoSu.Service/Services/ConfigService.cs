using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class ConfigService : IConfigService
    {
        private readonly ConfigRepository _cameraRepository = new();

        public async Task CreateCameraAsync(Config camera)
        {
            await _cameraRepository.AddCameraAsync(camera);
        }

        public async Task DeleteCameraAsync(Guid id)
        {
            await _cameraRepository.DeleteCameraAsync(id);
        }

        public async Task<Config> GetCameraAsync()
        {
            return await _cameraRepository.GetCameraAsync();
        }

        public async Task UpdateCameraAsync(Config camera)
        {
            await _cameraRepository.UpdateCameraAsync(camera);
        }

        public async Task<Config> GetCameraByIdAsync(Guid cameraId)
        {
            return await _cameraRepository.GetCameraByIdAsync(cameraId);
        }

        public async Task<Config> GetNewestCameraAsync()
        {
            return await _cameraRepository.GetNewestCameraAsync();
        }
    }
}
