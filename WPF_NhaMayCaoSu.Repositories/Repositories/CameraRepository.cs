using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private readonly CaoSuWpfDbContext _context;

        public CameraRepository(CaoSuWpfDbContext context)  // Ensure correct DbContext is used
        {
            _context = context;
        }

        public async Task AddCamera(Camera camera)
        {
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
            try
            {
                return await _context.Cameras.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error retrieving camera from the database", ex);
            }
        }

        public async Task UpdateCamera(Camera camera)
        {
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
            try
            {
                Camera camera = await GetCamera();
                if (camera != null)
                {
                    _context.Cameras.Remove(camera);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting camera from the database", ex);
            }
        }
    }
}
