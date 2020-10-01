using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.AdminPanel.Controls;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text;

namespace Buran.Core.MvcLibrary.AdminPanel
{
    public static class PageMenuExt
    {
        internal static void AddMenu(StringBuilder sb, List<EditorPageMenuSubItem> menu)
        {
            foreach (var item in menu)
            {
                var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                sb.AppendLine($@"<li><a href='{item.Url}' class='{item.ButtonClass}' {target}>{item.Title}</a></li>");
            }
        }

        internal static void AddMenuSubItem(StringBuilder sb, List<EditorPageMenuSubItem> menu)
        {
            foreach (var item in menu)
            {
                var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                sb.AppendLine($@"<a href='{item.Url}' class='dropdown-item {item.ButtonClass}' {target}>{item.Title}</a>");
            }
        }

        internal static void AddMenuSplit(StringBuilder sb, List<EditorPageMenuSplitItem> menu)
        {
            foreach (var item in menu)
            {
                var btnText = "";
                if (item.Id.IsEmpty())
                {
                    var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                    btnText = $@"<a href='{item.Url}' {target} class='dropdown-item {item.ButtonClass}'><span>{item.Title}</span></a>";
                }
                else
                {
                    btnText = $@"<a id='{item.Id}' href='#' class='dropdown-item {item.ButtonClass}'><span>{item.Title}</span></a>";
                }
                sb.AppendLine(btnText);
            }
        }

        public static HtmlString PageMenu(this IHtmlHelper helper, EditorPageMenu menu)
        {
            if (menu.Items.Count == 0)
            {
                return new HtmlString(string.Empty);
            }

            var sb = new StringBuilder();

            sb.AppendLine("<div class='hidden-xs'>");
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0)
                {
                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        sb.AppendLine($@"<a href='{item.Url}' class='{item.ButtonClass}' {target}>
    <i class='{item.IconClass}'></i> <span>{item.Title}</span>
</a>");
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";

                        sb.AppendLine($@"<button id='{item.Id}' class='{item.ButtonClass}' {pu} {cd}>
<i class='{item.IconClass}'></i> <span>{item.Title}</span>
</button>");
                    }
                }
                else
                {
                    sb.AppendLine($@"<div class='btn-group'>
<a class='btn btn-default {item.ButtonClass}' href='#' data-toggle='dropdown'>
    <i class='{item.IconClass}'></i> {item.Title} <i class='fa fa-angle-down'></i>
</a>
<ul class='dropdown-menu'>");
                    AddMenu(sb, item.Items);
                    sb.AppendLine("</ul></div>");
                }
            }
            sb.AppendLine("</div>");


            sb.AppendLine("<div class='visible-xs'>");
            sb.AppendLine($@"<div class='btn-group'>
<a class='btn btn-default {""}' href='#' data-toggle='dropdown'>
    <i class='{""}'></i> {"İşlemler"} <i class='fa fa-angle-down'></i>
</a>
<ul class='dropdown-menu'>");
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0)
                {
                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        sb.AppendLine($@"<li><a href='{item.Url}' class='{item.ButtonClass}' {target}>
    <i class='{item.IconClass}'></i> <span>{item.Title}</span>
</a></li>");
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";

                        sb.AppendLine($@"<li><button id='{item.Id}' class='btn btn-link {item.ButtonClass}' {pu} {cd}>
<i class='{item.IconClass}'></i> <span>{item.Title}</span>
</button></li>");
                    }
                }
            }
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            return new HtmlString(sb.ToString());
        }




        public static HtmlString PageMenu2(this IHtmlHelper helper, EditorPageMenu menu)
        {
            if (menu.Items.Count == 0)
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();
            sb.AppendLine("<ul class='navbar-nav mr-auto navbar-toolbar'>");
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0 && item.SplitItems.Count == 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    sb.AppendLine("<li class='nav-item'>");
                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        sb.AppendLine($@"<a href='{item.Url}' class='{item.ButtonClass} {item.ButtonIdClass}' {target}><i class='{item.IconClass}'></i> <span>{item.Title}</span></a>");
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";
                        sb.AppendLine($@"<button id='{item.Id}' class='{item.ButtonClass} {item.ButtonIdClass}' {pu} {cd}><i class='{item.IconClass}'></i> <span>{item.Title}</span></button>");
                    }
                    sb.AppendLine("</li>");
                }
                else if (item.Items.Count > 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    sb.AppendLine($@"<li class='nav-item dropdown'>
        <a class='dropdown-toggle {item.ButtonClass} {item.ButtonIdClass}' href='#' data-toggle='dropdown'>
            <i class='{item.IconClass}'></i>&nbsp;{item.Title}
        </a>
        <div class='dropdown-menu'>");
                    AddMenuSubItem(sb, item.Items);
                    sb.AppendLine("</div></li>");
                }
                else if (item.SplitItems.Count > 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    string btnText;
                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        btnText = $@"<a href='{item.Url}' class='{item.ButtonClass} {item.ButtonIdClass}' {target}><i class='{item.IconClass}'></i> <span>{item.Title}</span></a>";
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";
                        btnText = $@"<button id='{item.Id}' class='{item.ButtonClass} {item.ButtonIdClass}' {pu} {cd}><i class='{item.IconClass}'></i> <span>{item.Title}</span></button>";
                    }

                    sb.AppendLine($@"<li class='nav-item dropdown'>
<div class='btn-group'>
        {btnText}
        <button type='button' class='{item.ButtonClass} dropdown-toggle dropdown-toggle-split' data-toggle='dropdown'>
            <span class='sr-only'>Toggle Dropdown</span>
        </button>
        <div class='dropdown-menu'>");
                    AddMenuSplit(sb, item.SplitItems);
                    sb.AppendLine("</div></div></li>");
                }
            }
            sb.AppendLine("</ul>");
            return new HtmlString(sb.ToString());
        }

        public static HtmlString PageMenu2Mobile(this IHtmlHelper helper, EditorPageMenu menu)
        {
            if (menu.Items.Count == 0)
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();
            foreach (var item in menu.Items)
            {
                if (item.Items.Count == 0 && item.SplitItems.Count == 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        sb.AppendLine($@"<a href='{item.Url}' class='{item.ButtonClass}' {target}><i class='{item.IconClass}'></i> <span>{item.Title}</span></a>");
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";
                        sb.AppendLine($@"<button id='mitem-{item.Id}' class='{item.ButtonClass}' {pu} {cd}><i class='{item.IconClass}'></i> <span>{item.Title}</span></button>");
                    }
                }
                else if (item.Items.Count > 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    sb.AppendLine($@"<a class='dropdown-toggle {item.ButtonClass}' href='#' data-toggle='dropdown'>
            <i class='{item.IconClass}'></i>&nbsp;{item.Title}
        </a>
        <div class='dropdown-menu'>");
                    AddMenuSubItem(sb, item.Items);
                    sb.AppendLine("</div>");
                }
                else if (item.SplitItems.Count > 0)
                {
                    if (item.ButtonClass.IsEmpty())
                        item.ButtonClass = "btn btn-label-primary btn-sm";

                    string btnText;
                    if (item.Id.IsEmpty())
                    {
                        var target = item.Target.IsEmpty() ? "" : $" target='{item.Target}'";
                        btnText = $@"<a href='{item.Url}' class='{item.ButtonClass}' {target}><i class='{item.IconClass}'></i> <span>{item.Title}</span></a>";
                    }
                    else
                    {
                        var pu = "";
                        if (!item.PostUrl.IsEmpty())
                            pu = $"data-posturl='{item.PostUrl}'";
                        var cd = "";
                        if (!item.ConfirmText.IsEmpty())
                            cd = $"data-confirm='{item.ConfirmText}'";
                        btnText = $@"<button id='mitem-{item.Id}' class='{item.ButtonClass}' {pu} {cd}><i class='{item.IconClass}'></i> <span>{item.Title}</span></button>";
                    }

                    sb.AppendLine($@"<div class='btn-group'>
        {btnText}
        <button type='button' class='{item.ButtonClass} dropdown-toggle dropdown-toggle-split' data-toggle='dropdown'>
            <span class='sr-only'>Toggle Dropdown</span>
        </button>
        <div class='dropdown-menu'>");
                    AddMenuSplit(sb, item.SplitItems);
                    sb.AppendLine("</div></div>");
                }
            }
            return new HtmlString(sb.ToString());
        }
    }
}
