using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IImageService
    {
        Task AddImageAsync(Image image);
        Task<IEnumerable<Image>> GetAllImagesAsync(int pageNumber, int pageSize);
        Task<Image> GetImageByIdAsync(Guid imageId);
        Task<IEnumerable<Image>> GetImagesBySaleIdAsync(Guid saleId);
        Task<IEnumerable<Image>> Get2LatestImagesBySaleIdAsync(Guid saleId);
        Task UpdateImageAsync(Image image);
        Task DeleteImageAsync(Guid imageId);
    }
}
