using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Controller.Controller
{
    public class FirebaseController
    {
        private readonly string _bucketName = "nhamaycaosu-images.appspot.com";
        private readonly string _googleHeader = "https://storage.googleapis.com";
        public FirebaseController()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("../nhamaycaosu.json")
                });
            }
        }

        public async Task<string> UploadImageAsync(string localFilePath, string firebaseFileName)
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
                throw;
            }
        }

        public async Task DownloadImageAsync(string firebaseFileName, string localPath)
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
                throw;
            }
        }
    }
}
