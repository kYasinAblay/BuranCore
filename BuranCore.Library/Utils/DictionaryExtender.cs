using System.Collections.Generic;
using System.Text;

namespace Buran.Core.Library.Utils
{
    public static class DictionaryExtender
    {
        public static string ToEncode(this Dictionary<string, string> dict)
        {
            var sb = new StringBuilder();
            var firstPair = true;
            foreach (var kv in dict.Keys)
            {
                var name = kv;
                var value = dict[kv];
                if (!firstPair)
                {
                    sb.Append("&");
                }
                sb.Append(name).Append("=").Append(value);
                firstPair = false;
            }
            return sb.ToString();
        }
    }
}