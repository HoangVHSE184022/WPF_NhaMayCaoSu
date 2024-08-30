
namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IFirebaseService
    {
        Task<string?> SaveImagePathToDatabaseAsync(string localFilePath, string firebaseFileName);
        Task DownloadImageFromFirebaseAsync(string firebaseFileName, string localPath);
    }
}
