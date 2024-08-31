using System;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class CameraService : ICameraService
    {
        private readonly CameraRepository _cameraRepository = new();

        public async Task CreateCameraAsync(Camera camera)
        {
            await _cameraRepository.AddCameraAsync(camera);
        }

        public async Task DeleteCameraAsync(Guid id)
        {
            await _cameraRepository.DeleteCameraAsync(id);
        }

        public async Task<Camera> GetCameraAsync()
        {
            return await _cameraRepository.GetCameraAsync();
        }

        public async Task UpdateCameraAsync(Camera camera)
        {
            await _cameraRepository.UpdateCameraAsync(camera);
        }

        public async Task<Camera> GetCameraByIdAsync(Guid cameraId)
        {
            return await _cameraRepository.GetCameraByIdAsync(cameraId);
        }

        public async Task<Camera> GetNewestCameraAsync()
        {
            return await _cameraRepository.GetNewestCameraAsync();
        }
    }
}
