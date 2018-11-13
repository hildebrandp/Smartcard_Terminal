using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Drawing;
using QRCoder;

namespace smartcardSupport
{
    class qrCodeClass
    {
        private QRCodeGenerator qrGenerator;
        private QRCodeData qrCodeData;
        private QRCode qrCode;
        private Bitmap qrCodeImage;
        private Random rnd = new Random();

        private CryptLib _crypt;
        private String iv;
        private String key;
        private String password;
        private int pin = 0;

        private String btAddress;

        /// <summary>
        /// Class Constructor that creates crypto object
        /// </summary>
        public qrCodeClass()
        {
            _crypt = new CryptLib();
            qrGenerator = new QRCodeGenerator();
        }

        /// <summary>
        /// Method that generates QR-Code with given Text
        /// </summary>
        /// <param name="text">Text that will be written in QR-Code</param>
        /// <returns>Image object with QR-Code Image</returns>
        private Image genQRCode(String text)
        {
            qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            qrCode = new QRCode(qrCodeData);
            qrCodeImage = qrCode.GetGraphic(6);

            return qrCodeImage;
        }

        /// <summary>
        /// Method for gen. AES-256 key + Salt-128
        /// </summary>
        /// <param name="btAddress"></param>
        /// <returns>Image object with QR-Code Image</returns>
        public Image newQRCode(String btAddress)
        {
            password = genRandomPassword();
            iv = CryptLib.GenerateRandomIV(16); //16 bytes = 128 bits
            key = CryptLib.getHashSha256(password, 32); //32 bytes = 256 bits

            pin = rnd.Next(10000000, 100000000);

            this.btAddress = btAddress;

            return genQRCode(btAddress + ">>" + pin.ToString() + ">>" + key + ">>" + iv + ">>" + 0);
        }

        /// <summary>
        /// Method for gen. a random password for AES key
        /// </summary>
        /// <returns>String with random password</returns>
        private String genRandomPassword()
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);

                string token = Convert.ToBase64String(tokenData);
                return token;
            }
        }

        /// <summary>
        /// Getter Method for Salt
        /// </summary>
        /// <returns>AES-Salt</returns>
        public String getSalt()
        {
            return iv;
        }

        /// <summary>
        /// Getter Method for Key
        /// </summary>
        /// <returns>AES-Key</returns>
        public String getKey()
        {
            return key;
        }

        /// <summary>
        /// Getter Method for Authentication Pin
        /// </summary>
        /// <returns>Pin</returns>
        public int getPin()
        {
            return pin;
        }
    }
}
