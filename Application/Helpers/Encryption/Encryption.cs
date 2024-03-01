using System;
using System.Security.Cryptography;
using System.Text;


namespace Application.Helpers.Encryption
{
    public class Encryption : IEncryption
    {
        private static string Encryptkey = "3BA16347812D7";
        public string EncryptData(string textData)
        {
            RijndaelManaged RijObj = new RijndaelManaged
            {
                Mode = CipherMode.CBC,

                Padding = PaddingMode.PKCS7,

                KeySize = 0x80,

                BlockSize = 0x80
            };

            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptkey);

            byte[] EncryptkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int length = passBytes.Length;
            if (length > EncryptkeyBytes.Length)
            {
                length = EncryptkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptkeyBytes, length);

            RijObj.Key = EncryptkeyBytes;
            RijObj.IV = EncryptkeyBytes;


            ICryptoTransform CryptoObjtransform = RijObj.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);

            return Convert.ToBase64String(CryptoObjtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }

        public string DecryptData(string EncryptText)
        {
            RijndaelManaged RijObj = new RijndaelManaged();
            RijObj.Mode = CipherMode.CBC;
            RijObj.Padding = PaddingMode.PKCS7;

            RijObj.KeySize = 0x80;
            RijObj.BlockSize = 0x80;
            byte[] encryptTxtByte = Convert.FromBase64String(EncryptText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(Encryptkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = keyBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(keyBytes, EncryptionkeyBytes, len);
            RijObj.Key = EncryptionkeyBytes;
            RijObj.IV = EncryptionkeyBytes;
            byte[] TxtByte = RijObj.CreateDecryptor().TransformFinalBlock(encryptTxtByte, 0, encryptTxtByte.Length);
            return Encoding.UTF8.GetString(TxtByte);
        }

    }
}
