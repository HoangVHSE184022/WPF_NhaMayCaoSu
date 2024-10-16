using QRCoder;
using System;
using OtpNet;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.OTPService
{
    public class OTPServices
    {
        private readonly string _issuer;
        private readonly string _user;

        public OTPServices(string issuer, string user)
        {
            _issuer = issuer;
            _user = user;
        }

        /// <summary>
        /// Generate a secret key for the user.
        /// </summary>
        /// <returns>Base32 encoded secret key</returns>
        public string GenerateSecretKey()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(secretKey); // Return Base32 encoded secret key
        }

        /// <summary>
        /// Generates a URI to be used with Google Authenticator.
        /// </summary>
        /// <param name="secretKey">The secret key for generating the URI</param>
        /// <returns>The URI that can be scanned by Google Authenticator</returns>
        public string GenerateGoogleAuthUri(string secretKey)
        {
            return $"otpauth://totp/{_issuer}:{_user}?secret={secretKey}&issuer={_issuer}&digits=6";
        }

        /// <summary>
        /// Generates a QR code that can be scanned by Google Authenticator.
        /// </summary>
        /// <param name="googleAuthUri">The URI for Google Authenticator</param>
        /// <returns>A bitmap image of the QR code</returns>
        public Bitmap GenerateQRCode(string googleAuthUri)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(googleAuthUri, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20); // Adjust size here if needed
                return qrCodeImage;
            }
        }

        /// <summary>
        /// Verifies if the TOTP entered by the user is valid.
        /// </summary>
        /// <param name="secretKey">The secret key for generating the TOTP</param>
        /// <param name="userInputCode">The OTP entered by the user</param>
        /// <returns>True if the code is valid, false otherwise</returns>
        public bool VerifyTotp(string secretKey, string userInputCode)
        {
            byte[] secretKeyBytes = Base32Encoding.ToBytes(secretKey); // Decode the Base32 secret key
            Totp totp = new Totp(secretKeyBytes); // Generate TOTP using the secret key

            bool isValid = totp.VerifyTotp(userInputCode, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
            return isValid;
        }

        /// <summary>
        /// Saves the generated QR code image to a file.
        /// </summary>
        /// <param name="qrCodeImage">The QR code bitmap image</param>
        /// <param name="filePath">The file path where the image will be saved</param>
        public void SaveQrCodeImage(Bitmap qrCodeImage, string filePath)
        {
            qrCodeImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png); // Save as PNG
        }

        /// <summary>
        /// Converts the QR code to a byte array.
        /// </summary>
        /// <param name="qrCodeImage">The QR code bitmap image</param>
        /// <returns>A byte array of the QR code</returns>
        public byte[] ConvertQrCodeToBytes(Bitmap qrCodeImage)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
    }
}
