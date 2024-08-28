using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class CameraService : ICameraService
    {
        private readonly ICameraRepository _cameraRepository;

        public CameraService(ICameraRepository cameraRepository)
        {
            _cameraRepository = cameraRepository;
        }

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
    }
}
