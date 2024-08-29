using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Service.Interfaces
{
    public interface IFirebaseService
    {
        Task<string?> SaveImagePathToDatabaseAsync(string localFilePath, string firebaseFileName);
        Task DownloadImageFromFirebaseAsync(string firebaseFileName, string localPath);
    }
}
