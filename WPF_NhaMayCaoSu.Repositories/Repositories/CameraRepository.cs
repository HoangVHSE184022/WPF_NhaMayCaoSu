using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private CaoSuWpfDbContext _context;


        public async Task AddCameraAsync(Camera camera)
        {
            _context = new();
            await _context.Cameras.AddAsync(camera);
            await _context.SaveChangesAsync();
        }

        public async Task<Camera> GetCameraAsync()
        {
            _context = new();
            return await _context.Cameras.FirstOrDefaultAsync(c => c.Status == 1); 
        }

        public async Task UpdateCameraAsync(Camera camera)
        {
            _context = new();
            _context.Cameras.Update(camera);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCameraAsync(Guid id)
        {
            _context = new();
            Camera camera = await _context.Cameras.FindAsync(id);
            if (camera != null)
            {
                camera.Status = 0;
                _context.Cameras.Update(camera);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Camera> GetCameraByIdAsync(Guid cameraId)
        {
            _context = new();
            return await _context.Cameras.FirstOrDefaultAsync(c => c.CameraId == cameraId && c.Status == 1);
        }

        public async Task<Camera> GetNewestCameraAsync()
        {
            _context = new();
            return await _context.Cameras
                                 .Where(c => c.Status == 1)
                                 .OrderByDescending(c => c.CameraId) 
                                 .FirstOrDefaultAsync();
        }
    }
}
