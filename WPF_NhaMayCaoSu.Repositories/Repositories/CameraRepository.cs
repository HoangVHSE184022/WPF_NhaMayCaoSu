using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private CaoSuWpfDbContext _context;


        public async Task AddCamera(Camera camera)
        {
            _context = new();
            try
            {
                await _context.Cameras.AddAsync(camera);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error adding camera to the database", ex);
            }
        }

        public async Task<Camera> GetCamera()
        {
            _context = new();
            try
            {
                return await _context.Cameras.FirstOrDefaultAsync(c => c.Status == 1); // Retrieve only available cameras
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error retrieving camera from the database", ex);
            }
        }

        public async Task UpdateCamera(Camera camera)
        {
            _context = new();
            try
            {
                _context.Cameras.Update(camera);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error updating camera in the database", ex);
            }
        }

        public async Task DeleteCamera(Guid id)
        {
            _context = new();
            try
            {
                Camera camera = await _context.Cameras.FindAsync(id);
                if (camera != null)
                {
                    camera.Status = 0; // Mark as unavailable instead of deleting
                    _context.Cameras.Update(camera);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error marking camera as unavailable in the database", ex);
            }
        }

        public async Task<Camera> GetCameraById(Guid cameraId)
        {
            _context = new();
            try
            {
                return await _context.Cameras.FirstOrDefaultAsync(c => c.CameraId == cameraId && c.Status == 1);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving camera by ID from the database", ex);
            }
        }

        public async Task<Camera> GetNewestCamera()
        {
            _context = new();
            try
            {
                return await _context.Cameras
                    .Where(c => c.Status == 1)
                    .OrderByDescending(c => c.CameraId)  // Assuming CameraId is a sequential GUID or there is a DateCreated property
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving the newest camera from the database", ex);
            }
        }
    }
}
