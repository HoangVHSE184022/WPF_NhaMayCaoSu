using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly ImageRepository _repo = new();

        public async Task AddImageAsync(Image image)
        {
            await _repo.AddImageAsync(image);
        }

        public async Task<IEnumerable<Image>> GetAllImagesAsync(int pageNumber, int pageSize)
        {
            return await _repo.GetAllImagesAsync(pageNumber, pageSize);
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            return await _repo.GetImageByIdAsync(imageId);
        }

        public async Task<IEnumerable<Image>> GetImagesBySaleIdAsync(Guid saleId)
        {
            return await _repo.GetImagesBySaleIdAsync(saleId);
        }

        public async Task UpdateImageAsync(Image image)
        {
            await _repo.UpdateImageAsync(image);
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            await _repo.DeleteImageAsync(imageId);
        }

        public async Task<IEnumerable<Image>> Get2LatestImagesBySaleIdAsync(Guid saleId)
        {
            return await _repo.Get2LatestImagesBySaleIdAsync(saleId);
        }
    }
}
