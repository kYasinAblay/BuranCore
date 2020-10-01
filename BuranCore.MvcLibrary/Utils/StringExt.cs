using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Text.Encodings.Web;
using System.Web;
using Buran.Core.Library.Utils;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class StringExt
    {
        public static string JsAlert(string text)
        {
            return "alert(\"" + HttpUtility.JavaScriptStringEncode(text.Replace("\r\n", "")) + "\");";
        }

        public static string GetString(this TagBuilder tag)
        {
            var writer = new StringWriter();
            tag.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string GetString(this IHtmlContent content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string GetString(this IHtmlContentBuilder content)
        {
            var writer = new StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static long GetImageId(this StringValues t1)
        {
            var text = t1.ToString();
            if (!text.IsEmpty())
            {
                if (text.Contains(","))
                {
                    var t = text.Split(',');
                    return long.Parse(t[0]);
                }
                else
                    return long.Parse(text);
            }
            return 0;
        }
    }
}
