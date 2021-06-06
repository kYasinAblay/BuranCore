using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Buran.Core.Library.Utils
{
    public static class StringHashExtender
    {
        public static string GetMd5Hash(this string input)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            bytes = provider.ComputeHash(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static string GetMD5(this string sourceText)
        {
            byte[] textBytes = Encoding.Default.GetBytes(sourceText);
            try
            {
                var cryptHandler = new MD5CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                    {
                        ret += "0" + a.ToString("x");
                    }
                    else
                    {
                        ret += a.ToString("x");
                    }
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public static string GetSHA256Hash(this string input)
        {
            var provider = new SHA256CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            bytes = provider.ComputeHash(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static string GetSHA256(this string sourceText)
        {
            byte[] textBytes = Encoding.Default.GetBytes(sourceText);
            try
            {
                var cryptHandler = new SHA256CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                    {
                        ret += "0" + a.ToString("x");
                    }
                    else
                    {
                        ret += a.ToString("x");
                    }
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public static string GetSHA5126Hash(this string input)
        {
            var provider = new SHA512CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(input);
            bytes = provider.ComputeHash(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static string GetSHA512(this string sourceText)
        {
            byte[] textBytes = Encoding.Default.GetBytes(sourceText);
            try
            {
                var cryptHandler = new SHA512CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                    {
                        ret += "0" + a.ToString("x");
                    }
                    else
                    {
                        ret += a.ToString("x");
                    }
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public static string ToBase64(this string text)
        {
            var t = Encoding.ASCII.GetBytes(text);
            return Convert.ToBase64String(t);
        }

        public static string FromBase64(this string text)
        {
            var t = Convert.FromBase64String(text);
            return Encoding.ASCII.GetString(t);
        }

        public static string ToUtfBase64(this string text)
        {
            var t = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(t);
        }

        public static string FromUtfBase64(this string text)
        {
            var t = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(t);
        }
    }
}
