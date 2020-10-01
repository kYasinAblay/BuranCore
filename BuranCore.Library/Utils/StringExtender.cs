using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Buran.Core.Library.Utils
{
    public static class StringExtender
    {
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static string Fill(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string Join<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }

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

        public static string CleanHtml(this string text)
        {
            return text.Replace(@"<[^>]*>", string.Empty);
        }

        public static string CheckLastAnd(this string text)
        {
            return text.Last() == '&'
                ? text.Substring(0, text.Length - 1)
                : text;
        }

        public static string ReplaceStringTr(this string text)
        {
            var r = text.ToLower();
            r = r.Replace("ı", "i");
            r = r.Replace("ş", "s");
            r = r.Replace("ö", "o");
            r = r.Replace("ç", "c");
            r = r.Replace("ğ", "g");
            r = r.Replace("ü", "u");
            return r;
        }

        public static string ReplaceString(this string text)
        {
            var r = text.ToLower();
            r = r.Replace("ı", "i");
            r = r.Replace("ş", "s");
            r = r.Replace("ö", "o");
            r = r.Replace("ç", "c");
            r = r.Replace("ğ", "g");
            r = r.Replace("ü", "u");
            r = r.Replace("\"", "");
            r = r.Replace("\t", "");
            r = r.Replace("\r", "");
            r = r.Replace("\n", "");
            r = r.Replace(" ", "-");
            r = r.Replace(".", "");
            r = r.Replace(",", "");
            r = r.Replace("'", "");
            r = r.Replace("~", "");
            r = r.Replace(";", "");
            r = r.Replace("?", "");
            r = r.Replace("\\", "");
            r = r.Replace("/", "");
            r = r.Replace("!", "");
            r = r.Replace("^", "");
            r = r.Replace("#", "");
            r = r.Replace("+", "");
            r = r.Replace("$", "");
            r = r.Replace("%", "");
            r = r.Replace("&", "");
            r = r.Replace("{", "");
            r = r.Replace("}", "");
            r = r.Replace("(", "");
            r = r.Replace(")", "");
            r = r.Replace("[", "");
            r = r.Replace("]", "");
            r = r.Replace("=", "");
            r = r.Replace("*", "");
            r = r.Replace("@", "");
            r = r.Replace("`", "");
            r = r.Replace("´", "");
            return r;
        }

        public static string MakeAddress(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            text = Regex.Replace(text.ReplaceStringTr(), "[^A-Za-z0-9 -]", "");
            text = text.Replace(" ", "-");
            while (true)
            {
                if (text.IndexOf("--", StringComparison.Ordinal) > -1)
                {
                    text = text.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            text = text.Trim('-');
            return text.Replace("-html", ".html");
        }

        public static string FirstCharToUpper(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return text.First().ToString(CultureInfo.InvariantCulture).ToUpper() + String.Join("", text.Skip(1));
            }

            return text;
        }

        public static string ToShort(this string text, int length, string prefix = "...")
        {
            return !string.IsNullOrEmpty(text)
                       ? (text.Length > length ? text.Substring(0, length - prefix.Length) + prefix : text)
                       : text;
        }

        public static bool IsNumeric(this string value)
        {
            return value.All(Char.IsNumber);
        }

        public static string MakeShort(this string hash)
        {
            return hash.Substring(0, 12) + "...";
        }

        public static string TelefonTemizle(this string text)
        {
            var a = "";
            foreach (var t in text)
            {
                if (char.IsDigit(t))
                    a += t;
            }
            return a;
        }

        public static int ToInt(this string value)
        {
            if (value.IsEmpty())
                return 0;
            var v = value.Replace(".", "");
            int.TryParse(v, out int i);
            return i;
        }

        public static decimal ToDecimal(this string value)
        {
            decimal? retVal = null;
            try
            {
                var cu = new CultureInfo("tr-TR");
                retVal = decimal.Parse(value.ToString(), cu);
            }
            catch
            {

            }
            return retVal ?? 0;
        }

        public static long ToLong(this string value)
        {
            if (value.IsEmpty())
                return 0;
            var v = value.Replace(".", "");
            long.TryParse(v, out long i);
            return i;
        }

        public static string GetMonthName(this int t)
        {
            var mn = "";
            if (t == 1)
                mn = "Ocak";
            else if (t == 2)
                mn = "Şubat";
            else if (t == 3)
                mn = "Mart";
            else if (t == 4)
                mn = "Nisan";
            else if (t == 5)
                mn = "Mayıs";
            else if (t == 6)
                mn = "Haziran";
            else if (t == 7)
                mn = "Temmuz";
            else if (t == 8)
                mn = "Ağustos";
            else if (t == 9)
                mn = "Eylül";
            else if (t == 10)
                mn = "Ekim";
            else if (t == 11)
                mn = "Kasım";
            else if (t == 12)
                mn = "Aralık";
            return mn;
        }

        public static string NextCell(this string text)
        {
            var last = text.Last();
            last++;
            if (last == '[')
            {
                last = 'A';
                if (text.Length == 1)
                {
                    return "A" + last;
                }

                var first2 = text.First();
                first2++;
                return first2.ToString() + last;
            }
            if (text.Length == 1)
            {
                return last.ToString();
            }
            var first = text.First();
            return first.ToString() + last;
        }

        public static string MaskCC(this string serial)
        {
            return serial.Substring(0, 6) + "******" + serial.Substring(serial.Length - 4);
        }
    }
}
