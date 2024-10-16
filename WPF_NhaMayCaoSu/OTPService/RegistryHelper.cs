using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WPF_NhaMayCaoSu.OTPService
{
    public class RegistryHelper
    {
        private const string RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\CaoSuApp";
        private const string DemoStartDateKey = "DemoStartDate";
        private const string IsUnlockedKey = "IsUnlocked";
        private const string SecretKey = "SecretKey"; // Key for the secret

        private string Encrypt(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] key = Encoding.UTF8.GetBytes("CaoSuAmazingTechActivationKey");
            using (var sha256 = SHA256.Create())
            {
                key = sha256.ComputeHash(key);
            }

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        private string Decrypt(string cipherText)
        {
            byte[] data = Convert.FromBase64String(cipherText);
            byte[] key = Encoding.UTF8.GetBytes("CaoSuAmazingTechActivationKey");
            using (var sha256 = SHA256.Create())
            {
                key = sha256.ComputeHash(key);
            }

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                using (var ms = new MemoryStream(data))
                {
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, iv.Length);
                    using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] decrypted = new byte[data.Length - iv.Length];
                            cs.Read(decrypted, 0, decrypted.Length);
                            return Encoding.UTF8.GetString(decrypted).TrimEnd('\0');
                        }
                    }
                }
            }
        }

        public void SetDemoStartDate(DateTime startDate)
        {
            string encryptedDate = Encrypt(startDate.ToString("o"));
            Registry.SetValue(RegistryPath, DemoStartDateKey, encryptedDate);
        }

        public DateTime? GetDemoStartDate()
        {
            object value = Registry.GetValue(RegistryPath, DemoStartDateKey, null);
            if (value != null)
            {
                string decryptedDate = Decrypt(value.ToString());
                return DateTime.Parse(decryptedDate);
            }
            return null;
        }

        public void SetUnlocked()
        {
            string encryptedUnlocked = Encrypt("True");
            Registry.SetValue(RegistryPath, IsUnlockedKey, encryptedUnlocked);
        }

        public bool IsUnlocked()
        {
            object value = Registry.GetValue(RegistryPath, IsUnlockedKey, null);
            if (value != null)
            {
                string decryptedValue = Decrypt(value.ToString());
                return decryptedValue == "True";
            }
            return false;
        }

        public void SetSecretKey(string secretKey)
        {
            string encryptedKey = Encrypt(secretKey);
            Registry.SetValue(RegistryPath, SecretKey, encryptedKey);
        }

        public string GetSecretKey()
        {
            object value = Registry.GetValue(RegistryPath, SecretKey, null);
            if (value != null)
            {
                return Decrypt(value.ToString());
            }
            return null;
        }
    }
}
