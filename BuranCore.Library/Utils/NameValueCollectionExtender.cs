using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Buran.Core.Library.Utils
{
    public static class NameValueCollectionExtender
    {
        public static string ToQueryString(this NameValueCollection dict)
        {
            return ToQueryString(dict, (string[])null);
        }

        public static string ToQueryString(this NameValueCollection dict, string removeKey)
        {
            return ToQueryString(dict, new[] { removeKey });
        }

        public static string ToQueryString(this NameValueCollection dict, string[] removeKeys)
        {
            var sb = new StringBuilder();
            var firstPair = true;
            var keys = dict.AllKeys;
            if (removeKeys != null)
            {
                var list = removeKeys.Select(key => key.ToLowerInvariant()).ToList();
                keys = keys.Where(d => d != null && !list.Contains(d.ToLowerInvariant())).ToArray();
            }
            foreach (var kv in keys)
            {
                var name = kv;
                var values = dict.GetValues(name);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        if (value.IsEmpty())
                        {
                            continue;
                        }

                        if (!firstPair)
                        {
                            sb.Append("&");
                        }

                        sb.Append(name).Append("=").Append(value);
                        firstPair = false;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
