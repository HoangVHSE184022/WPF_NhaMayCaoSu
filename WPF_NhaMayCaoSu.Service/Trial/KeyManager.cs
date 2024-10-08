using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace WPF_NhaMayCaoSu.Service.Trial
{
    public class KeyManager
    {
        private const string ValidHash = "AmazingTechCaoSuHash";
        private const string RegistryKeyPath = @"Software\YourAppName";
        private const string ActivationKey = "ActivationKey";
        private const string IsActivatedKey = "IsActivated";

        // Validate the entered key using SHA256 hash
        public bool ValidateKey(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hash == ValidHash;
            }
        }

        // Save the activation key and activation status
        public void SaveActivation(string key)
        {
            using (var regKey = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
            {
                regKey.SetValue(IsActivatedKey, true);  // Save activation status
                regKey.SetValue(ActivationKey, key);    // Save activation key
            }
        }

        // Check if the app is activated
        public bool IsActivated()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (regKey != null)
                {
                    return regKey.GetValue(IsActivatedKey) as bool? ?? false;
                }
            }
            return false;
        }

        // Retrieve the stored license key
        public string GetStoredKey()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (regKey != null)
                {
                    return regKey.GetValue(ActivationKey) as string;
                }
            }
            return null;
        }
    }
}
