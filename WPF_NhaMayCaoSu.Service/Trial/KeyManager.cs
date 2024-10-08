using Microsoft.Win32;

namespace WPF_NhaMayCaoSu.Service.Trial
{
    public class KeyManager
    {
        private const string ValidKey = "AmazingTechCaosuActivate";  // Define the valid plain key
        private const string RegistryKeyPath = @"Software\CaoSuApp";
        private const string ActivationKey = "ActivationKey";
        private const string IsActivatedKey = "IsActivated";

        // Validate the entered key by comparing it with the plain key
        public bool ValidateKey(string input)
        {
            return input == ValidKey;  // Compare input key with the valid plain key
        }

        // Save the activation key and activation status
        public void SaveActivation(string key)
        {
            using (var regKey = Registry.CurrentUser.CreateSubKey(RegistryKeyPath))
            {
                regKey.SetValue(IsActivatedKey, 1);  // Store activation status as 1 (true)
                regKey.SetValue(ActivationKey, key);  // Save the activation key
            }
        }

        // Check if the app is activated
        public bool IsActivated()
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                if (regKey != null)
                {
                    var isActivatedValue = regKey.GetValue(IsActivatedKey);

                    // Handle case where the value is stored as an int (DWORD) or string
                    if (isActivatedValue is int)  // If it's stored as an int (1 = activated)
                    {
                        return (int)isActivatedValue == 1;
                    }
                    else if (isActivatedValue is string)  // If it's stored as a string
                    {
                        return bool.TryParse(isActivatedValue as string, out bool isActivated) && isActivated;
                    }
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
                    return regKey.GetValue(ActivationKey) as string;
                }
            }
            return null;
        }
    }
}
