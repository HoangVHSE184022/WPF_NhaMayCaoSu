using MongoDB.Driver;
using System.Diagnostics;

namespace WPF_NhaMayCaoSu.Service.Dependencies
{
    public class MongoDbConfiguration
    {
        public IMongoClient MongoClient { get; private set; }
        public string DatabaseName { get; private set; } = "QuanLyKeyCaoSuDb";

        public MongoDbConfiguration()
        {
            MongoClient = new MongoClient("mongodb+srv://trinhhoangnhatminh123:aBQ028Brfg4CMWpB@cluster0.ksmzr.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
        }

        // Method to test the MongoDB connection
        public void TestConnection()
        {
            try
            {
                Debug.WriteLine("MongoDB connection successful.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MongoDB connection failed: {ex.Message}");
            }
        }
    }
}
