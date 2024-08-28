using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface ICameraService
    {
        Task CreateCamera(Camera camera);
        Task<Camera> GetCamera();
        Task UpdateCamera(Camera camera);
        Task DeleteCamera(Guid id);
    }
}
