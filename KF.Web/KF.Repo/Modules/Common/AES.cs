using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Common
{
    public class AES
    {
        public static string encryptionKey = System.Configuration.ConfigurationSettings.AppSettings["encryptionKey"];
        public static string encryptionIV = System.Configuration.ConfigurationSettings.AppSettings["encryptionIV"];
        private Byte[] keyBytes = UTF8Encoding.UTF8.GetBytes(encryptionKey);
        private Byte[] IvBytes = UTF8Encoding.UTF8.GetBytes(encryptionIV);

        private static RijndaelManaged GetCryptoAlgorithm()
        {
            RijndaelManaged algorithm = new RijndaelManaged();
            //set the mode, padding and block size
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Mode = CipherMode.CBC;
            algorithm.KeySize = 128;
            algorithm.BlockSize = 128;
            return algorithm;
        }

        public string AesEncrypt(string inputText)
        {
            byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(inputText);//AbHLlc5uLone0D1q

            string result = null;
            MemoryStream memoryStream = new MemoryStream();

            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, GetCryptoAlgorithm().CreateEncryptor(keyBytes, IvBytes), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                cryptoStream.FlushFinalBlock();
                result = Convert.ToBase64String(memoryStream.ToArray());
            }
            return result;
        }

        public string AesDecrypt(string cipherText)
        {
            Byte[] outputBytes = System.Convert.FromBase64String(cipherText); ;
            string plaintext = string.Empty;
            MemoryStream memoryStream = new MemoryStream(outputBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, GetCryptoAlgorithm().CreateDecryptor(keyBytes, IvBytes), CryptoStreamMode.Read);
            using (StreamReader srDecrypt = new StreamReader(cryptoStream))
            {
                plaintext = srDecrypt.ReadToEnd();
            }
            return plaintext;
        }
    }
}
