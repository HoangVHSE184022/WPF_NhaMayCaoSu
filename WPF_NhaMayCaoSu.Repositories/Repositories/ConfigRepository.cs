using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private CaoSuWpfDbContext _context;


        public async Task AddCameraAsync(Config camera)
        {
            _context = new();
            await _context.Cameras.AddAsync(camera);
            await _context.SaveChangesAsync();
        }

        public async Task<Config> GetCameraAsync()
        {
            _context = new();
            return await _context.Cameras.FirstOrDefaultAsync(c => c.Status == 1);
        }

        public async Task UpdateCameraAsync(Config camera)
        {
            _context = new();
            _context.Cameras.Update(camera);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCameraAsync(Guid id)
        {
            _context = new();
            Config camera = await _context.Cameras.FindAsync(id);
            if (camera != null)
            {
                camera.Status = 0;
                _context.Cameras.Update(camera);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Config> GetCameraByIdAsync(Guid cameraId)
        {
            _context = new();
            return await _context.Cameras.FirstOrDefaultAsync(c => c.CameraId == cameraId && c.Status == 1);
        }

        public async Task<Config> GetNewestCameraAsync()
        {
            _context = new();
            return await _context.Cameras
                                 .Where(c => c.Status == 1)
                                 .OrderByDescending(c => c.CameraId)
                                 .FirstOrDefaultAsync();
        }
    }
}
