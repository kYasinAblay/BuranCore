using System;
using System.IO;

namespace Buran.Core.Library.Utils
{
    public class ImageExtender
    {
        public static string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(File.ReadAllBytes(fileName));
        }
    }
}
