using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using WPF_NhaMayCaoSu.Service.Interfaces;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly string _bucketName = "nhamaycaosu-images.appspot.com";
        private readonly string _googleHeader = "https://storage.googleapis.com";

        public FirebaseService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("../nhamaycaosu.json")
                });
            }
        }

        public async Task<string?> SaveImagePathToDatabaseAsync(string localFilePath, string firebaseFileName)
        {
            try
            {
                StorageClient storage = StorageClient.Create();

                using FileStream fileStream = File.OpenRead(localFilePath);
                var storageObject = await storage.UploadObjectAsync(_bucketName, firebaseFileName, null, fileStream);

                string imageUrl = $"{_googleHeader}/{_bucketName}/{firebaseFileName}";
                return imageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file to Firebase Storage: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadImageFromFirebaseAsync(string firebaseFileName, string localPath)
        {
            try
            {
                StorageClient storage = StorageClient.Create();

                using var outputFile = File.OpenWrite(localPath);
                await storage.DownloadObjectAsync(_bucketName, firebaseFileName, outputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file from Firebase Storage: {ex.Message}");
            }
        }
    }
}
