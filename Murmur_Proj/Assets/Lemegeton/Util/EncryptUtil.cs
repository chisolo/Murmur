using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lemegeton
{
    public static class EncryptUtil
    {
        public static string MD5(string input)
        {
            if(string.IsNullOrEmpty(input))
                return null;

            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sb = new StringBuilder();
            var len = output.Length;
            for(int i = 0; i < len; ++i) {
                sb.Append(output[i].ToString("x2"));
            }

            return sb.ToString();
        }
        public static string DESEncrypt(string input, string key)
        {
            if(string.IsNullOrEmpty(key)) return input;
            using(DESCryptoServiceProvider des = new DESCryptoServiceProvider()) {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(input);
                MemoryStream ms = new MemoryStream();
                using(CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(key)), CryptoStreamMode.Write)) {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                }
                ms.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string DESDecrypt(string input, string key)
        {
            if(string.IsNullOrEmpty(key)) return input;
            using(DESCryptoServiceProvider des = new DESCryptoServiceProvider()) {
                byte[] inputByteArray = Convert.FromBase64String(input);
                MemoryStream ms = new MemoryStream();
                using(CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(key)), CryptoStreamMode.Write)) {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                }
                ms.Close();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
