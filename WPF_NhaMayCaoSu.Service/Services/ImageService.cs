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
            await _repo.AddImage(image);
        }

        public async Task<IEnumerable<Image>> GetAllImagesAsync(int pageNumber, int pageSize)
        {
            return await _repo.GetAllImages(pageNumber, pageSize);
        }

        public async Task<Image> GetImageByIdAsync(Guid imageId)
        {
            return await _repo.GetImageById(imageId);
        }

        public async Task<IEnumerable<Image>> GetImagesBySaleIdAsync(Guid saleId)
        {
            return await _repo.GetImagesBySaleId(saleId);
        }

        public async Task UpdateImageAsync(Image image)
        {
            await _repo.UpdateImage(image);
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            await _repo.DeleteImage(imageId);
        }
    }
}
