using System;
using System.Globalization;

namespace Buran.Core.Library.Utils
{
    public static class DecimalExtender
    {
        public static string ToInt(this decimal value)
        {
            return value.ToString("N0", new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "" });
        }

        public static string ToGlobalText(this double value)
        {
            return value.ToString("N8", new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "" });
        }

        public static string ToGlobalText(this decimal value)
        {
            return value.ToString("N2", new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "" });
        }

        public static string ToGlobalText(this decimal value, int decimalPart)
        {
            return value.ToString("N" + decimalPart, new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "" });
        }

        public static string ToText(this decimal value)
        {
            return value.ToString("N2");
        }

        public static string YaziyaCevir(this decimal tutar)
        {
            var sTutar = tutar.ToString("F2").Replace('.', ',');
            var lira = sTutar.Substring(0, sTutar.IndexOf(','));
            var kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);
            var yazi = "";

            string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };
            string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
            string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" };

            int grupSayisi = 6;
            lira = lira.PadLeft(grupSayisi * 3, '0');
            string grupDegeri;
            for (int i = 0; i < grupSayisi * 3; i += 3)
            {
                grupDegeri = "";
                if (lira.Substring(i, 1) != "0")
                    grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ";
                if (grupDegeri == "BİRYÜZ")
                    grupDegeri = "YÜZ";
                grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))];
                grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))];
                if (grupDegeri != "")
                    grupDegeri += binler[i / 3];
                if (grupDegeri == "BİRBİN")
                    grupDegeri = "BİN";
                yazi += grupDegeri;
            }

            if (yazi != "")
                yazi += " TL ";
            int yaziUzunlugu = yazi.Length;

            if (kurus.Substring(0, 1) != "0")
                yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];
            if (kurus.Substring(1, 1) != "0")
                yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];

            if (yazi.Length > yaziUzunlugu)
                yazi += " Kr.";
            else
                yazi += "SIFIR Kr.";
            return yazi;
        }
    }
}
