using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static partial class AdminExtenders
    {
        public static HtmlString InfoPanel(this IHtmlHelper helper, string title, string desc = "")
        {
            return new HtmlString(string.Format(@"<div class='col-md-2'>
        <div class='portlet-info'>
            <h4>{0}</h4>
            <div class='desc'>{1}</div>
        </div>
    </div>", title, desc));
        }

        public static HtmlString PanelTitle(this IHtmlHelper helper, string title)
        {
            return new HtmlString($@"
<div class='portlet__head'>
    <div class='portlet__head-label'>
        <h3 class='portlet__head-title'>
            {title}
        </h3>
    </div>
</div>
");
        }
    }
}