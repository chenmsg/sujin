using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ITOrm.Core.Helper
{
    /// <summary>
    /// 加密/解密帮助类
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// 类测试
        /// </summary>
        public static void Test(string data = "Lump Ma")
        {
            System.Console.WriteLine("加密字符串是:" + data);
            string str = "";
            System.Console.WriteLine("DES3 CBC模式(加密/解密):");
            str = EncryptionHelper.Des3CbcEncode(data);
            System.Console.WriteLine("加密结果:" + str);
            System.Console.WriteLine("解密结果:" + EncryptionHelper.Des3CbcDecode(str));


            System.Console.WriteLine("DES3 ECB模式(加密/解密):");
            str = EncryptionHelper.Des3EcbEncode(data);
            System.Console.WriteLine("加密结果:" + str);
            System.Console.WriteLine("解密结果:" + EncryptionHelper.Des3EcbDecode(str));


            System.Console.WriteLine("AES模式(加密/解密):");
            str = EncryptionHelper.AESEncrypt(data);
            System.Console.WriteLine("加密结果:" + str);
            System.Console.WriteLine("解密结果:" + EncryptionHelper.AESDecrypt(str));


            System.Console.WriteLine("DES模式(加密/解密):");
            str = EncryptionHelper.DesEncrypt(data);
            System.Console.WriteLine("加密结果:" + str);
            System.Console.WriteLine("解密结果:" + EncryptionHelper.DesDecrypt(str));


            System.Console.WriteLine("带密钥和加密向量加解密,用Guid做Key和IV:");
            str = EncryptionHelper.Encrypt(data);
            System.Console.WriteLine("加密结果:" + str);
            System.Console.WriteLine("解密结果:" + EncryptionHelper.Decrypt(str));

        }

        #region MD5加密

        /// <summary>
        /// 获得32位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_32(this string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得16位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_16(this string input)
        {
            return input.MD5_32().Substring(8, 16);
        }

        /// <summary>
        /// 获得8位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5_8(this string input)
        {
            return input.MD5_32().Substring(8, 8);
        }

        #endregion

        #region DES3 CBC模式(加密/解密)
        /// <summary>
        /// 加密[DES3 CBC模式加密]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Des3CbcEncode(string data)
        {
            byte[] key = Convert.FromBase64String("YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4");
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };//当模式为ECB时，IV无用
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            byte[] result = Des3CbcEncode(key, iv, byteData);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解密[DES3 CBC模式解密]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Des3CbcDecode(string data)
        {
            byte[] key = Convert.FromBase64String("YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4");
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };//当模式为ECB时，IV无用
            byte[] byteData = Convert.FromBase64String(data);
            byte[] result = Des3CbcDecode(key, iv, byteData);
            //  \0是字符串的结尾标志，存储在字符串的结尾。你转换时字符串长度不够8位，系统默认加上\0来填充。
            return Encoding.UTF8.GetString(result).Replace("\0", "");
        }

        /// <summary>
        /// 加密[DES3 CBC模式加密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        static byte[] Des3CbcEncode(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;//默认值
                tdsp.Padding = PaddingMode.PKCS7;//默认值
                CryptoStream cStream = new CryptoStream(mStream, tdsp.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                byte[] ret = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// 解密[DES3 CBC模式解密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV</param>
        /// <param name="data">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        static byte[] Des3CbcDecode(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.CBC;
                tdsp.Padding = PaddingMode.PKCS7;
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, tdsp.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] fromEncrypt = new byte[data.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        #endregion

        #region DES3 ECB模式(加密/解密)

        /// <summary>
        /// 加密[DES3 ECB模式加密]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Des3EcbEncode(string data)
        {
            byte[] key = Convert.FromBase64String("YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4");
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };//当模式为ECB时，IV无用
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            byte[] result = Des3EcbEncode(key, iv, byteData);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解密[DES3 ECB模式解密]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Des3EcbDecode(string data)
        {
            byte[] key = Convert.FromBase64String("YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4");
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };//当模式为ECB时，IV无用
            byte[] byteData = Convert.FromBase64String(data);
            byte[] result = Des3EcbDecode(key, iv, byteData);
            //  \0是字符串的结尾标志，存储在字符串的结尾。你转换时字符串长度不够8位，系统默认加上\0来填充。
            return Encoding.UTF8.GetString(result).Replace("\0", "");
        }

        /// <summary>
        /// 加密[DES3 ECB模式加密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        static byte[] Des3EcbEncode(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                CryptoStream cStream = new CryptoStream(mStream, tdsp.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                byte[] result = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return result;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// 解密[DES3 ECB模式解密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        static byte[] Des3EcbDecode(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, tdsp.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] fromEncrypt = new byte[data.Length];
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }

        #endregion

        #region AES模式(加密/解密)

        /// <summary>
        /// 加密[AES加密]
        /// </summary>
        /// <param name="data">被加密的明文</param>
        /// <returns>结果</returns>
        public static string AESEncrypt(string data)
        {
            string key = "AWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4";//须是32位英文或数字
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 7, 8, 9, 8, 7, 4, 1, 7, 8, 7, 4 };
            string result = AESEncrypt(key, iv, data);
            return result;
        }

        /// <summary>
        /// 解密[AES解密]
        /// </summary>
        /// <param name="cipherText">被解密的密文</param>
        /// <returns>结果</returns>
        public static string AESDecrypt(string data)
        {
            string key = "AWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4";//须是32位英文或数字
            byte[] iv = new byte[] { 1, 2, 3, 4, 5, 7, 8, 9, 8, 7, 4, 1, 7, 8, 7, 4 };
            string result = AESDecrypt(key, iv, data);
            return result;
        }

        /// <summary>
        /// 加密[AES加密]
        /// </summary>
        /// <param name="key">密钥32位英文或数字</param>
        /// <param name="iv">IV 16位</param>
        /// <param name="data">被加密的明文</param>
        /// <returns>结果</returns>
        static string AESEncrypt(string key, byte[] iv, string data)
        {
            byte[] byteKey = new byte[32];
            Array.Copy(Encoding.Unicode.GetBytes(key.PadRight(byteKey.Length)), byteKey, byteKey.Length);
            byte[] dataByte = Encoding.UTF8.GetBytes(data);
            byte[] resultByte;//解密后的明文
            Rijndael aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流
                using (var memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (var encryptor = new CryptoStream(memory, aes.CreateEncryptor(byteKey, iv), CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        encryptor.Write(dataByte, 0, dataByte.Length);
                        encryptor.FlushFinalBlock();
                        resultByte = memory.ToArray();
                    }
                }
            }
            catch
            {
                resultByte = null;
            }

            if (resultByte != null)
            {
                return Convert.ToBase64String(resultByte);
            }
            return null;
        }

        /// <summary>
        /// 解密[AES解密]
        /// </summary>
        /// <param name="key">密钥32位英文或数字</param>
        /// <param name="iv">IV 16位</param>
        /// <param name="data">被解密的密文</param>
        /// <returns>结果</returns>
        static string AESDecrypt(string key, byte[] iv, string data)
        {
            byte[] byteKey = new byte[32];
            Array.Copy(Encoding.Unicode.GetBytes(key.PadRight(byteKey.Length)), byteKey, byteKey.Length);
            byte[] dataByte = Convert.FromBase64String(data);
            byte[] resultByte;//解密后的明文
            Rijndael aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文
                using (var memory = new MemoryStream(dataByte))
                {
                    // 把内存流对象包装成加密流对象
                    using (var decryptor = new CryptoStream(memory,
                    aes.CreateDecryptor(byteKey, iv),
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
                            resultByte = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                resultByte = null;
            }

            if (resultByte != null)
            {
                return Encoding.UTF8.GetString(resultByte);
            }

            return null;
        }

        #endregion

        #region DES模式(加密/解密)

        /// <summary>
        /// 加密[字符串DES加密]
        /// </summary>
        /// <param name="data">被加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string DesEncrypt(string data)
        {
            return DesEncrypt("abcEfsdeadewrfe", data);
        }

        /// <summary>
        /// 解密[字符串DES解密]
        /// </summary>
        /// <param name="data">被解密字符串</param>
        /// <returns>解密后字符串</returns>
        public static string DesDecrypt(string data)
        {
            return DesDecrypt("abcEfsdeadewrfe", data);
        }


        /// <summary>
        /// 加密[字符串DES加密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="data">被加密字符串</param>
        /// <returns>加密后字符串</returns>
        static string DesEncrypt(string key, string data)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                provider.IV = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(data);
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.FlushFinalBlock();
                StringBuilder builder = new StringBuilder();
                foreach (byte num in stream.ToArray())
                {
                    builder.AppendFormat("{0:X2}", num);
                }
                stream.Close();
                return builder.ToString();
            }
            catch (Exception) { return "xxxx"; }
        }

        /// <summary>
        /// 解密[字符串DES解密]
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="data">被解密字符串</param>
        /// <returns>解密后字符串</returns>
        static string DesDecrypt(string key, string data)
        {
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                provider.Key = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                provider.IV = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] buffer = new byte[data.Length / 2];
                for (int i = 0; i < (data.Length / 2); i++)
                {
                    int num2 = Convert.ToInt32(data.Substring(i * 2, 2), 0x10);
                    buffer[i] = (byte)num2;
                }
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.FlushFinalBlock();
                stream.Close();
                return Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
            }
            catch (Exception) { return ""; }
        }

        #endregion

        #region 带密钥和加密向量加解密,用Guid做Key和IV

        /// <summary>
        /// 加密[指定字符串,用Guid做Key和IV]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            string key = "aa898537-d70d-4512-8f7d-8d2420aa3969";//提前生成的Guid字符串
            string iv = "51f11181-dc2e-4f7a-8028-532a26210915";//提前生成的Guid字符串
            Guid guid1 = new Guid(key);
            Guid guid2 = new Guid(iv);
            return Encrypt(guid1, guid2, data);
        }

        /// <summary>
        /// 解密[指定的字符串,用Guid做Key和IV]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decrypt(string data)
        {
            string key = "aa898537-d70d-4512-8f7d-8d2420aa3969";//提前生成的Guid字符串
            string iv = "51f11181-dc2e-4f7a-8028-532a26210915";//提前生成的Guid字符串
            return Decrypt(key, iv, data);
        }

        /// <summary>
        /// 加密[指定字符串,用Guid做Key和IV]
        /// </summary>
        /// <param name="g1">加密key</param>
        /// <param name="g2">加密向量</param>
        /// <param name="data">源字符串</param>
        /// <returns>加密后的字符串</returns>
        static string Encrypt(Guid guidKey, Guid guidIV, string data)
        {
            byte[] clearBytes = Encoding.UTF8.GetBytes(data);
            byte[] key = guidKey.ToByteArray();
            byte[] iv = guidIV.ToByteArray();
            MemoryStream mem = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(mem, tdes.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            encStream.Write(clearBytes, 0, clearBytes.Length);
            encStream.FlushFinalBlock();
            byte[] result = new byte[mem.Length];
            Array.Copy(mem.GetBuffer(), 0, result, 0, mem.Length);
            string myResult = BitConverter.ToString(result, 0);
            return myResult;
        }

        /// <summary>
        /// 解密[指定的字符串,用Guid做Key和IV]
        /// </summary>
        /// <param name="strKey">解密key</param>
        /// <param name="strIV">解密向量</param>
        /// <param name="data">被加密的字符串</param>
        /// <returns>字符串原文</returns>
        static string Decrypt(string strKey, string strIV, string data)
        {
            Guid guid1 = new Guid(strKey);
            Guid guid2 = new Guid(strIV);
            byte[] key = guid1.ToByteArray();
            byte[] iv = guid2.ToByteArray();
            string[] toRecover = data.Split(new char[] { '-' });
            byte[] br = new byte[toRecover.Length];
            for (int i = 0; i < toRecover.Length; i++)
            {
                br[i] = byte.Parse(toRecover[i], NumberStyles.HexNumber);
            }
            MemoryStream mem = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(mem, tdes.CreateDecryptor(key, iv), CryptoStreamMode.Write);
            encStream.Write(br, 0, br.Length);
            encStream.FlushFinalBlock();
            byte[] result = new byte[mem.Length];
            Array.Copy(mem.GetBuffer(), 0, result, 0, mem.Length);
            return Encoding.UTF8.GetString(result, 0, result.Length);
        }

        #endregion
    }
}
