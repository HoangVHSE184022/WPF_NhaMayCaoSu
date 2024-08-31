using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private CaoSuWpfDbContext _context;

        public async Task AddImageAsync(Image image)
        {
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Image>> GetAllImagesAsync(int pageNumber, int pageSize)
        {
            return await _context.Images
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(img => img.ImageId == imageId);
        }

        public async Task<IEnumerable<Image>> GetImagesBySaleIdAsync(Guid saleId)
        {
            return await _context.Images
                                 .Where(img => img.SaleId == saleId)
                                 .ToListAsync();
        }

        public async Task UpdateImageAsync(Image image)
        {
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            Image image = await _context.Images.FindAsync(imageId);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
