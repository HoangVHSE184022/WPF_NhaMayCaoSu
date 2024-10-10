using Microsoft.Win32;

namespace WPF_NhaMayCaoSu.Service.Trial
{
    public class KeyManager
    {
        private const string ValidKey = "AmazingTechCaosuActivate";
        private const string RegistryKeyPath = @"Software\CaoSuApp";
        private const string ActivationKey = "ActivationKey";
        private const string ActivationHashKey = "ActivationHash";

        // Validate the entered key by comparing it with the plain key
        public bool ValidateKey(string input)
        {
            return input == ValidKey;  // Compare input key with the valid plain key
        }

        // Save the activation key and a hashed activation status
        public void SaveActivation(string key)
        {
            using (var regKey = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
            {
                // Encrypt the activation key
                string encryptedKey = EncryptionHelper.Encrypt(key);
                regKey.SetValue(ActivationKey, encryptedKey);  // Save the encrypted activation key

                // Save a hash indicating that the app is activated
                string hashValue = HashHelper.ComputeHash("Activated");
                regKey.SetValue(ActivationHashKey, hashValue);
            }
        }

        // Check if the app is activated
        public bool IsActivated()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (regKey != null)
                {
                    var storedHash = regKey.GetValue(ActivationHashKey) as string;
                    var currentHash = HashHelper.ComputeHash("Activated");
                    return storedHash == currentHash;  // Compare hashes
                }
            }
            return false;  // Default to false if the key is not found or invalid
        }

        // Retrieve the stored license key
        public string GetStoredKey()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (regKey != null)
                {
                    string encryptedKey = regKey.GetValue(ActivationKey) as string;
                    return encryptedKey != null ? EncryptionHelper.Decrypt(encryptedKey, EncryptionHelper.Key, EncryptionHelper.IV) : null;
                }
            }
            return null;
        }
    }
}
