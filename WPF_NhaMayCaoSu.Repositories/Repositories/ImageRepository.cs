using Microsoft.EntityFrameworkCore;
using WPF_NhaMayCaoSu.Repository.Context;
using WPF_NhaMayCaoSu.Repository.IRepositories;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private CaoSuWpfDbContext _context;

        public ImageRepository()
        {
            _context = new CaoSuWpfDbContext();
        }

        public async Task AddImage(Image image)
        {
            _context = new CaoSuWpfDbContext();
            await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Image>> GetAllImages(int pageNumber, int pageSize)
        {
            _context = new CaoSuWpfDbContext();

            return await _context.Images
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<Image> GetImageById(Guid imageId)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.Images.FirstOrDefaultAsync(img => img.ImageId == imageId);
        }

        public async Task<IEnumerable<Image>> GetImagesBySaleId(Guid saleId)
        {
            _context = new CaoSuWpfDbContext();
            return await _context.Images
                                 .Where(img => img.SaleId == saleId)
                                 .ToListAsync();
        }

        public async Task UpdateImage(Image image)
        {
            _context = new CaoSuWpfDbContext();
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImage(Guid imageId)
        {
            _context = new CaoSuWpfDbContext();
            Image image = await _context.Images.FindAsync(imageId);
            if (image != null)
            {
                _context.Images.Update(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
