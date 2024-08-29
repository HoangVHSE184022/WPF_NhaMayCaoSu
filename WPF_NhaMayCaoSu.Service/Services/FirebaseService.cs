using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_NhaMayCaoSu.Controller.Controller;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly FirebaseController _firebaseController = new();
        public async Task<string?> SaveImagePathToDatabaseAsync(string localFilePath, string firebaseFileName)
        {
            try
            {
                string imageUrl = await _firebaseController.UploadImageAsync(localFilePath, firebaseFileName);

                return imageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadImageFromFirebaseAsync(string firebaseFileName, string localPath)
        {
            try
            {
                await _firebaseController.DownloadImageAsync(firebaseFileName, localPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
