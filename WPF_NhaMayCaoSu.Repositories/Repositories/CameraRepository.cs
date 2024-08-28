using System;
using System.Collections.Generic;
using System.Linq;
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

        public void AddCamera(Camera camera)
        {
            try
            {
                _context.Cameras.Add(camera);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error adding camera to the database", ex);
            }
        }

        public Camera GetCamera()
        {
            try
            {
                return _context.Cameras.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error retrieving camera from the database", ex);
            }
        }

        public IEnumerable<Camera> GetAllCameras()
        {
            try
            {
                return _context.Cameras.ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error retrieving all cameras from the database", ex);
            }
        }

        public void UpdateCamera(Camera camera)
        {
            try
            {
                _context.Cameras.Update(camera);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log error and/or rethrow
                throw new Exception("Error updating camera in the database", ex);
            }
        }

        public void DeleteCamera(Guid id)
        {
            try
            {
                var camera = GetCamera();
                if (camera != null)
                {
                    _context.Cameras.Remove(camera);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting camera from the database", ex);
            }
        }
    }
}
