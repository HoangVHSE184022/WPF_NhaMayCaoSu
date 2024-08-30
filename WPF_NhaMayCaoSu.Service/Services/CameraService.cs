using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class CameraService : ICameraService
    {
        private readonly CameraRepository _cameraRepository = new();

        public async Task CreateCamera(Camera camera)
        {
            await _cameraRepository.AddCamera(camera);
        }

        public async Task DeleteCamera(Guid id)
        {
            await _cameraRepository.DeleteCamera(id);
        }

        public async Task<Camera> GetCamera()
        {
            return await _cameraRepository.GetCamera();
        }

        public async Task UpdateCamera(Camera camera)
        {
            await _cameraRepository.UpdateCamera(camera);
        }

        public async Task<Camera> GetCameraById(Guid cameraId)
        {
            return await _cameraRepository.GetCameraById(cameraId);
        }

        public async Task<Camera> GetNewestCamera()
        {
            return await _cameraRepository.GetNewestCamera();
        }
    }
}
