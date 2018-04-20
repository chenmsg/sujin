using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ITOrm.Utility.Encryption
{
   public  class SecurityHelper
   {
       private static readonly string Key = SiteSettingsHelper.Instance.EncryptionKey;

       private static readonly byte[] Iv = new byte[] { 0x73, 0x7c, 0x72, 
                                                     0x7b, 0xf2, 0x6b, 
                                                     0x6f, 0xc5, 0x30, 
                                                     0x01, 0x67, 0x2b, 
                                                     0xfe, 0xd7, 0xab, 
                                                     0x76 };
       /// <summary>
       /// AES加密
       /// </summary>
       /// <param name="plainText">被加密的明文</param>
       /// <returns></returns>
       public static byte[] AESEncrypt(byte[] plainText)
       {
           var bKey = new Byte[32];
           Array.Copy(Encoding.Unicode.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

           Byte[] cipherTextBytes; // 加密后的密文

           var aes = Rijndael.Create();
           try
           {
               // 开辟一块内存流
               using (var memory = new MemoryStream())
               {
                   // 把内存流对象包装成加密流对象
                   using (var encryptor = new CryptoStream(memory,
                   aes.CreateEncryptor(bKey, Iv),
                   CryptoStreamMode.Write))
                   {
                       // 明文数据写入加密流
                       encryptor.Write(plainText, 0, plainText.Length);
                       encryptor.FlushFinalBlock();

                       cipherTextBytes = memory.ToArray();
                   }
               }
           }
           catch
           {
               cipherTextBytes = null;
           }

           return cipherTextBytes;
       }

       /// <summary>
       /// AES加密
       /// </summary>
       /// <param name="plainText">被加密的明文</param>
       /// <returns></returns>
       public static string AESEncrypt(string plainText)
       {
           var plainTextByte = Encoding.UTF8.GetBytes(plainText);
           var cipherText = AESEncrypt(plainTextByte);
           var text = Convert.ToBase64String(cipherText);
           return text;
       }

       /// <summary>
       /// AES解密
       /// </summary>
       /// <param name="cipherText">被解密的密文</param>
       /// <returns></returns>
       public static byte[] AESDecrypt(byte[] cipherText)
       {
           var bKey = new Byte[32];
           Array.Copy(Encoding.Unicode.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

           Byte[] plainTextByte; // 解密后的明文

           var aes = Rijndael.Create();
           try
           {
               // 开辟一块内存流，存储密文
               using (var memory = new MemoryStream(cipherText))
               {
                   // 把内存流对象包装成加密流对象
                   using (var decryptor = new CryptoStream(memory,
                   aes.CreateDecryptor(bKey, Iv),
                   CryptoStreamMode.Read))
                   {
                       // 明文存储区
                       using (var originalMemory = new MemoryStream())
                       {
                           var buffer = new Byte[1024];
                           int readBytes;
                           while ((readBytes = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                           {
                               originalMemory.Write(buffer, 0, readBytes);
                           }

                           plainTextByte = originalMemory.ToArray();
                       }
                   }
               }
           }
           catch
           {
               plainTextByte = null;
           }

           return plainTextByte;
       }

       /// <summary>
       /// AES解密
       /// </summary>
       /// <param name="cipherText">被解密的密文</param>
       /// <returns></returns>
       public static string AESDecrypt(string cipherText)
       {
           if (String.IsNullOrEmpty(cipherText)) return cipherText;

           cipherText = cipherText.Replace(" ", "+");
           var cipherTextByte = Convert.FromBase64String(cipherText);
           var plainText = AESDecrypt(cipherTextByte);
           var text = Encoding.UTF8.GetString(plainText);
           return text;
       }
        #region 取得MD5加密串
        /// 取得字符串的md5加密串 Mike Cheers 20120103添加
        /// <summary>
        /// 取得字符串的md5加密串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="encodingString">编码集,如GB2312等</param>
        /// <returns></returns>
        static public string GetMD5String(string str, Encoding encoding)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encoding.GetBytes(str));
            StringBuilder reString = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                reString.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return reString.ToString();
        }

        /// 取得字符串的md5加密串 Mike Cheers 20120103添加
        /// <summary>
        /// 取得字符串的md5加密串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <returns></returns>
        static public string GetMD5String(string str)
        {
            return GetMD5String(str, Encoding.UTF8);
        }
        static public string GetMd5StringGb2312(string str)
        {
            return GetMD5String(str, Encoding.GetEncoding("gb2312"));
        }
        /// 取得字符串的md5加密串 Blueice20111103添加
        /// Updated By Mike Cheers 20120103更新
        /// <summary>
        /// 取得字符串的md5加密串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="encodingString">编码集,如GB2312等</param>
        /// <returns></returns>
        static public string GetMD5String(string str, string encodingString)
        {
            return GetMD5String(str, Encoding.GetEncoding(encodingString));
        }

        static public string GetMD5String(string str, int codePage)
        {
            return GetMD5String(str, Encoding.GetEncoding(codePage));
        }


        #endregion
    }
}
