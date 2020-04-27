using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GTIFramework.Common.Utils.Converters
{
    public class EncryptionConvert
    {
        //AES 암호화를 위한 키. 임의값. 암/복호화에 필요.
        static string PasswordKey = "kyu";

        /// <summary>
        /// BASE64 암호화
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string Base64Encoding(string strInput)
        {
            var encodeBytes = Encoding.UTF8.GetBytes(strInput);
            return Convert.ToBase64String(encodeBytes);
        }

        /// <summary>
        /// BASE64 복호화
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static string Base64Decoding(string strBase64)
        {
            var encodedBytes = Convert.FromBase64String(strBase64);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        /// <summary>
        /// PW 암호화 SHA256
        /// </summary>
        public static string SHA256Hash(string str)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(str));

            StringBuilder sbSHA256 = new StringBuilder();
            foreach (byte b in hash)
            {
                sbSHA256.AppendFormat("{0:x2}", b);
            }
            return sbSHA256.ToString();
        }

        /// <summary>
        /// AES (암호화)
        /// 사용안함
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AESEncryption(string str)
        {
            byte[] inputText = System.Text.Encoding.Unicode.GetBytes(str);
            byte[] passwordSalt = Encoding.ASCII.GetBytes(PasswordKey.Length.ToString());

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(PasswordKey, passwordSalt);

            Rijndael rijAlg = Rijndael.Create();
            rijAlg.Key = secretKey.GetBytes(32);
            rijAlg.IV = secretKey.GetBytes(16);

            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            MemoryStream msEncrypt = new MemoryStream();

            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(inputText, 0, inputText.Length);
            csEncrypt.FlushFinalBlock();

            byte[] encryptBytes = msEncrypt.ToArray();

            msEncrypt.Close();
            csEncrypt.Close();

            // Base64
            string encryptedData = Convert.ToBase64String(encryptBytes);

            return encryptedData;
        }

        /// <summary>
        /// AES (복호화)
        /// 사용안함
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AESDescryption(string str)
        {
            byte[] encryptedData = Convert.FromBase64String(str);

            byte[] passwordSalt = Encoding.ASCII.GetBytes(PasswordKey.Length.ToString());

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(PasswordKey, passwordSalt);

            Rijndael rijAlg = Rijndael.Create();

            rijAlg.Key = secretKey.GetBytes(32);

            rijAlg.IV = secretKey.GetBytes(16);

            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            MemoryStream msDecrypt = new MemoryStream(encryptedData);

            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            int decryptedCount = csDecrypt.Read(encryptedData, 0, encryptedData.Length);

            msDecrypt.Close();
            csDecrypt.Close();

            // Base64
            string decryptedData = Encoding.Unicode.GetString(encryptedData, 0, decryptedCount);

            return decryptedData;

        }
    }
}
