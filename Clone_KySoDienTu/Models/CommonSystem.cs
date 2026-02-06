using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Clone_KySoDienTu
{
    public class Kieudulieu
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class QueryString
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class CommonSystem
    {
        public static int _itemsofpage = 20;
        private static List<Kieudulieu> _kieu = new List<Kieudulieu>(new Kieudulieu[]{new Kieudulieu(){ key= "text", value ="Kiểu ký tự"},
                                new Kieudulieu(){ key="calendar", value ="Kiểu ngày"},
                                new Kieudulieu(){key="select", value ="Kiểu chọn"},
                                new Kieudulieu(){key="textarea", value ="Kiểu văn bản"},
                                new Kieudulieu(){key="autocomplete", value ="Từ điển"},
                                new Kieudulieu(){key="number", value ="Kiểu số"}});
        public static List<Kieudulieu> KieuDulieu
        {
            get { return _kieu; }
        }
        public static List<QueryString> getQueryString(string pas)
        {
            string qrystring = CryptData.querydecrypt(pas);
            string[] qrys = qrystring.Split('&');
            List<QueryString> res = new List<QueryString>();
            foreach (string qry in qrys)
            {
                string[] v = qry.Split('=');
                if (v.Length > 1)
                    res.Add(new QueryString() { Key = v[0], Value = v[1] });
            }
            return res;
        }
    }
    public static class CryptData
    {
        //byte[] lbtVector = { 240, 3, 45, 29, 0, 76, 173, 59 };
        static byte[] lbtVector = { 240, 3, 45, 29, 0, 76, 173, 59 };
        static private string lscryptoKey = "Key123Password!";



        static public string encrypt(string sSource)
        {
            TripleDESCryptoServiceProvider loCryptoClass = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider loCryptoProvider = new MD5CryptoServiceProvider();
            byte[] lbtBuffer;
            try
            {
                lbtBuffer = System.Text.Encoding.ASCII.GetBytes(sSource);
                loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey));
                loCryptoClass.IV = lbtVector;
                return Convert.ToBase64String(loCryptoClass.CreateEncryptor().TransformFinalBlock(lbtBuffer, 0, lbtBuffer.Length));
            }
            catch (CryptographicException)
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                loCryptoClass.Clear();
                loCryptoProvider.Clear();
                loCryptoClass = null;
                loCryptoProvider = null;
            }
            return "";
        }

        static public string queryencrypt(string sSource)
        {
            string result = "";
            if (sSource == null) return result;
            if (sSource == "") return result;
            return System.Web.HttpUtility.UrlEncode(encrypt(sSource).Replace("+", "@"));
        }

        static public string decrypt(string sSource)
        {
            byte[] buffer;
            TripleDESCryptoServiceProvider loCryptoClass = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider loCryptoProvider = new MD5CryptoServiceProvider();

            try
            {
                buffer = Convert.FromBase64String(sSource);
                loCryptoClass.Key = loCryptoProvider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(lscryptoKey));
                loCryptoClass.IV = lbtVector;
                return Encoding.ASCII.GetString(loCryptoClass.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (CryptographicException)
            {
            }
            catch (Exception)
            {
            }
            finally
            {
                loCryptoClass.Clear();
                loCryptoProvider.Clear();
                loCryptoClass = null;
                loCryptoProvider = null;
            }
            return "";
        }

        static public string querydecrypt(string sSource)
        {
            string result = "";
            if (sSource == null) return result;
            if (sSource == "") return result;
            result = System.Web.HttpUtility.UrlDecode(sSource).Replace("@", "+").Replace(" ", "+");
            return decrypt(result);
        }

        static public string CreateRandomPassword(int PasswordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            Byte[] randomBytes = new Byte[PasswordLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            char[] chars = new char[PasswordLength];
            int allowedCharCount = allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
    }
}