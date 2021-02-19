using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.XmlLang
{
    public static class XmlLangHtmlExtender
    {
        public static string L(string text)
        {
            if (text.IsEmpty())
                return string.Empty;
            return XmlLangResource.GetResource(text);
        }

        public static HtmlString L(this IHtmlHelper html, string text)
        {
            if (text.IsEmpty())
                return new HtmlString(string.Empty);
            return new HtmlString(XmlLangResource.GetResource(text));
        }

        public static HtmlString L(this IHtmlHelper html, string text, params object[] arg)
        {
            if (text.IsEmpty() || arg == null)
                return new HtmlString(string.Empty);
            var keyValue = XmlLangResource.GetResource(text);
            if (arg == null)
                return new HtmlString(keyValue);
            else
                return new HtmlString(string.Format(keyValue, arg));
        }
    }
}
