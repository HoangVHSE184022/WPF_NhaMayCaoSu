using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IImageRepository
    {
        Task AddImageAsync(Image image);
        Task<IEnumerable<Image>> GetAllImagesAsync(int pageNumber, int pageSize);
        Task<Image> GetImageByIdAsync(Guid imageId);
        Task<IEnumerable<Image>> GetImagesBySaleIdAsync(Guid saleId);
        Task UpdateImageAsync(Image image);
        Task DeleteImageAsync(Guid imageId);
    }
}
