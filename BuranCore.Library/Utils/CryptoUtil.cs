using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Buran.Core.Library.Utils
{
    public class RsaKey
    {
        public string Public { get; set; }
        public string Private { get; set; }
    }

    public class CryptoUtil
    {
        public byte[] AesEncrypt(string text, string key, string salt,
            int blockSize = 128, int keySize = 128, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            var saltData = Encoding.UTF8.GetBytes(salt);

            using (Aes cryp = Aes.Create())
            {
                cryp.BlockSize = blockSize;
                cryp.KeySize = keySize;
                cryp.Mode = mode;
                cryp.Key = keyData;
                cryp.IV = saltData;
                cryp.Padding = padding;

                byte[] encrypted;
                ICryptoTransform encryptor = cryp.CreateEncryptor(cryp.Key, cryp.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
                return encrypted;
            }
        }

        public string AesDecrypt(byte[] data, string key, string salt,
             int blockSize = 128, int keySize = 128, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            var saltData = Encoding.UTF8.GetBytes(salt);

            using (Aes cryp = Aes.Create())
            {
                cryp.BlockSize = blockSize;
                cryp.KeySize = keySize;
                cryp.Mode = mode;
                cryp.Key = keyData;
                cryp.IV = saltData;
                cryp.Padding = padding;

                string plaintext = null;
                ICryptoTransform decryptor = cryp.CreateDecryptor(cryp.Key, cryp.IV);
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
                return plaintext;
            }
        }


        public RsaKey GetRsaKey(int keySize = 4096)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                try
                {
                    var pp = rsa.ToXmlString(true);
                    var pu = rsa.ToXmlString(false);
                    var b64pp = pp.ToBase64();
                    var b64pu = pu.ToBase64();
                    var rk = new RsaKey { Public = b64pu, Private = b64pp };
                    return rk;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public string RsaEncrypt(string text, string publicKey, int keySize = 4096)
        {
            var textData = Encoding.UTF8.GetBytes(text);
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                try
                {
                    rsa.FromXmlString(publicKey.FromBase64());
                    var encryptedData = rsa.Encrypt(textData, true);
                    return Convert.ToBase64String(encryptedData);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public string RsaDecrypt(string text, string privateKey, int keySize = 4096)
        {
            try
            {
                var textData = Encoding.UTF8.GetBytes(text);
                using (var rsa = new RSACryptoServiceProvider(keySize))
                {
                    try
                    {
                        rsa.FromXmlString(privateKey.FromBase64());
                        var resultBytes = Convert.FromBase64String(text);
                        var decryptedBytes = rsa.Decrypt(resultBytes, true);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                    finally
                    {
                        rsa.PersistKeyInCsp = false;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public string RandomPass(int size, int numericLength)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                System.Threading.Thread.Sleep(10);
                builder.Append(ch);
            }
            for (var i = 0; i < numericLength; i++)
            {
                var r = random.Next(0, 9);
                builder.Append(r.ToString(CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }
    }
}
