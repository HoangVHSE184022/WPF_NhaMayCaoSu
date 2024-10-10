using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Service.Trial
{
    public class HashHelper
    {
        private static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("AmazingTechActivationSecret");

        public static string ComputeHash(string data)
        {
            using (var hmac = new HMACSHA256(SecretKey))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
