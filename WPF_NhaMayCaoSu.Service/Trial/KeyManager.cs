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
