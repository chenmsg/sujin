using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ITOrm.Payment.Masget

{
    public class Base64Method
    {
        public static string EncryptBase64(string a_strString)
        {
            string result = "";
            try
            {
                byte[] bt = Encoding.GetEncoding("utf-8").GetBytes(a_strString);
                MemoryStream ms = new MemoryStream();
                ms.Write(bt, 0, bt.Length);
                ms.Close();
                result = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.Replace('+', '-').Replace('/', '_');

        }

        public static string EncryptBase64(byte[] bt)
        {
            string result = "";
            try
            {
              
                MemoryStream ms = new MemoryStream();
                ms.Write(bt, 0, bt.Length);
                ms.Close();
                result = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.Replace('+', '-').Replace('/', '_');

        }

        public static string DecryptBase64(string a_strString)
        {
            string result = "";
            try
            {
                byte[] Buffer;
               
                    Buffer = Convert.FromBase64String(a_strString.Replace('-', '+').Replace('_', '/'));
                
              
                result = Encoding.GetEncoding("utf-8").GetString(Buffer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public static byte[] DecryptBase64ForByte(string a_strString)
        {
            string result = "";
            try
            {
                byte[] Buffer;

                Buffer = Convert.FromBase64String(a_strString.Replace('-', '+').Replace('_', '/'));


                return Buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }
    }
}
