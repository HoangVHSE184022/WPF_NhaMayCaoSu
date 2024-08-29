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
                var storage = StorageClient.Create();

                using var fileStream = File.OpenRead(localFilePath);
                var storageObject = await storage.UploadObjectAsync(_bucketName, firebaseFileName, null, fileStream);

                string imageUrl = $"https://storage.googleapis.com/{_bucketName}/{firebaseFileName}";
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
                var storage = StorageClient.Create();

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
