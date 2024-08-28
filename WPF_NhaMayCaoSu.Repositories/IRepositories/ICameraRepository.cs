using System.Collections.Generic;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface ICameraRepository
    {
        void AddCamera(Camera camera);
        Camera GetCamera();
        IEnumerable<Camera> GetAllCameras();
        void UpdateCamera(Camera camera);
        void DeleteCamera(Guid id);
    }
}