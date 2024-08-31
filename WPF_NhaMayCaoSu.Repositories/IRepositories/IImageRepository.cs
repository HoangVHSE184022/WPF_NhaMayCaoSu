using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.IRepositories
{
    public interface IImageRepository
    {
        Task AddImage(Image image);
        Task<IEnumerable<Image>> GetAllImages(int pageNumber, int pageSize);
        Task<Image> GetImageById(Guid imageId);
        Task<IEnumerable<Image>> GetImagesBySaleId(Guid saleId);
        Task UpdateImage(Image image);
        Task DeleteImage(Guid imageId);
    }
}
