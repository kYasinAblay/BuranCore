using System;
using System.Text;

namespace Buran.Core.Library.Utils
{
    public static class ByteExtender
    {
        private static readonly byte[] EmptyByteArray = new byte[0];
        private static byte[] HexMap = new byte[0x67] {
    0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x67, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67, 0x67,
    0x67, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
};

        public static string ByteArrayToHexString(this byte[] buf)
        {
            if (buf == null)
            {
                return "";
            }

            var hex = new StringBuilder(buf.Length * 2);
            foreach (byte b in buf)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(this string value)
        {
            if (value.IsEmpty())
            {
                throw new ArgumentNullException("value");
            }

            int length = value.Length;
            if ((length & 1) != 0)
            {
                throw new ArgumentException("Hex string must contain an even number of characters.", "value");
            }

            byte[] output = new byte[length >> 1];
            for (int i = 0; i < value.Length; i += 2)
            {
                uint c1 = value[i];
                uint c2 = value[i + 1];
                if ((c1 >= 0x67) || ((c1 = HexMap[c1]) >= 0x67) || (c2 >= 0x67) || ((c2 = HexMap[c2]) >= 0x67))
                {
                    throw new ArgumentException("Hex string contains unrecognized character.", "value");
                }

                output[i >> 1] = (byte)((c1 << 4) | c2);
            }
            return output;
        }
    }
}
