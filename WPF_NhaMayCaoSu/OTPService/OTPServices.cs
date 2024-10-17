using System;
using System.Drawing;
using System.IO;
using OtpNet;
using QRCoder;

namespace WPF_NhaMayCaoSu.OTPService
{
    public class OTPServices
    {
        private readonly string _issuer;
        private readonly string _user;

        // Shared secret key for all instances of the app (Base32 encoded)
        private const string SharedSecretKey = "JBSWY3DPEHPK3PXP"; 

        public OTPServices(string issuer, string user)
        {
            _issuer = issuer;
            _user = user;
        }

        /// <summary>
        /// Use the predefined shared secret key instead of generating a new one.
        /// </summary>
        /// <returns>The shared Base32 encoded secret key</returns>
        public string GenerateSecretKey()
        {
            // Always return the shared secret key
            return SharedSecretKey;
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
    }
}
