using Buran.Core.Library.Reflection;
using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Grid;
using Buran.Core.MvcLibrary.Grid.Columns;
using Buran.Core.MvcLibrary.Grid.Helper;
using Buran.Core.MvcLibrary.Grid.Pager;
using Buran.Core.MvcLibrary.Grid4.Helper4;
using Buran.Core.MvcLibrary.Grid4.Pager4;
using Buran.Core.MvcLibrary.Resource;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Buran.Core.MvcLibrary.Grid4
{
    public static class DataGridHelper4
    {
        public static int TotalRowCount { get; private set; }
        public static int TotalPageCount { get; private set; }

        public static HtmlString DataGrid4<T>(this IHtmlHelper helper, IEnumerable<T> data, DataColumn[] columns, DataGridOptions4 option)
            where T : class
        {
            var _query = helper.ViewContext.HttpContext.Request.QueryString;
            var _queryDictionary = QueryHelpers.ParseQuery(_query.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var _queryParams = _query.ToUriComponent();

            if (!option.PagerAndShortAction.IsEmpty())
            {
                var psss = option.PagerAndShortAction.Split('?');
                if (psss.Length > 1)
                    _queryParams = _queryParams.Replace("?" + psss[1], "");
            }

            var _colCount = 0;
            var items = data;
            if (items == null)
            {
                var content = new HtmlContentBuilder().AppendHtml(option.EmptyData);
                return new HtmlString(content.GetString());
            }

            var _sorter = new Sorter(_queryItems, option.SortKeyword, option.PagerAndShortAction, helper);
            var _filter = new Filter4(_queryDictionary, _queryItems, option.PagerKeyword,
                     helper.ViewContext.RouteData, option.PagerAndShortAction,
                     helper, option.PagerJsFunction, option.GridDiv);

            if (option.FilteringEnabled && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var kp = _filter.GetWhereString();
                if (kp.Where != null)
                    items = items.AsQueryable().Where(kp.Where, kp.Params.ToArray());
            }

            #region SORT DATA
            if (option.Sortable && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                if (_sorter.List.Count == 0 && option.DefaultSorting)
                {
                    if (!option.SortDefaultFieldName.IsEmpty())
                        _sorter.List.Add(new SorterInfo() { Direction = option.SortDefaultDirection, Keyword = option.SortDefaultFieldName });
                    if (option.SortDefaultFieldNames != null)
                    {
                        foreach (string sortinfodefaultname in option.SortDefaultFieldNames)
                        {
                            _sorter.List.Add(new SorterInfo()
                            {
                                Direction = option.SortDefaultDirection,
                                Keyword = sortinfodefaultname
                            });
                        }
                    }
                }
                var sorting = string.Empty;
                foreach (var info in _sorter.List)
                {
                    if (!sorting.IsEmpty())
                        sorting += ",";
                    sorting += String.Format("{0} {1}", info.Keyword, info.Direction);
                }
                if (!sorting.IsEmpty())
                    items = items.AsQueryable().OrderBy(sorting);
            }
            #endregion

            #region DO PAGING
            var currentPageSize = option.DefaultPageSize;
            var currentPageIndexItem = _queryItems.FirstOrDefault(d => d.Key == option.PagerKeyword);
            var pageIndex = 0;
            if (!currentPageIndexItem.Value.IsEmpty())
            {
                int.TryParse(currentPageIndexItem.Value, out pageIndex);
            }
            if (option.PagerEnabled && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var currentPageSizeStr = _queryItems.FirstOrDefault(d => d.Key == option.PageSizeKeyword);
                if (!currentPageSizeStr.Value.IsEmpty())
                {
                    int.TryParse(currentPageSizeStr.Value, out currentPageSize);
                }
                items = new PagedList<T>(items, pageIndex, currentPageSize);
                TotalRowCount = (items as PagedList<T>).TotalItemCount;
                TotalPageCount = (items as PagedList<T>).PageCount;
            }
            #endregion

            T firstItem = default(T);
            if (items != null && items.Any())
                firstItem = items.First();
            Type firstItemType = null;
            if (firstItem != null)
                firstItemType = firstItem.GetType();

            var writer = new HtmlContentBuilder();
            var baseUrl = LibGeneral.GetContentUrl(helper.ViewContext.RouteData);
            var fs = $@"/{baseUrl}/{option.PagerAndShortAction}{_queryParams}";
            var _refreshUrl = $"{option.PagerJsFunction}(\"{fs}\",\"{option.GridDiv}\");";

            if (option.PagerEnabled && (option.PagerLocation == PagerLocationTypes.Top || option.PagerLocation == PagerLocationTypes.TopAndBottom))
            {
                if (data is PagedList<T> || data is StaticPagedList<T>)
                {
                    var pager = RenderPager(helper, data, option, currentPageSize, _queryItems);
                    writer.AppendHtmlLine(pager);
                }
                else
                {
                    var pager = RenderPager(helper, items, option, currentPageSize, _queryItems);
                    writer.AppendHtmlLine(pager);
                }
            }
            writer.AppendHtmlLine($"<div id='dataGrid-{option.GridDiv}' data-refreshurl='{_refreshUrl}'> ");
            var cssTable = string.Empty;
            if (!option.CssTable.IsEmpty())
                cssTable = $" class=\"{option.CssTable}\"";
            var idTable = string.Empty;
            if (!option.TableId.IsEmpty())
                idTable = $" id=\"{option.TableId}\"";

            writer.AppendHtmlLine(RenderHeaderBar2(option));

            writer.AppendHtmlLine($"<table{idTable}{cssTable}>");

            if (option.ShowHeader)
            {
                writer.AppendHtmlLine($"<thead>");
                _colCount = columns.Count(d => d.Visible);
                if (option.ButtonDeleteEnabled || option.ButtonEditEnabled || option.ButtonRefreshEnabled)
                    _colCount++;
                if (option.FilteringEnabled)
                    writer.AppendHtmlLine(RenderFilterRow(helper, columns, firstItemType, _filter));
                writer.AppendHtmlLine(RenderHeader<T>(helper, columns, option, firstItemType, _refreshUrl, _sorter, _filter));
                writer.AppendHtmlLine($"</thead>");
            }

            writer.AppendHtmlLine($"<tbody>");
            foreach (var item in items)
                writer.AppendHtmlLine(RenderRow(helper, columns, item, option));

            writer.AppendHtmlLine($"</tbody>");
            writer.AppendHtmlLine("</table>");
            writer.AppendHtmlLine("</div>");

            if (option.FilteringEnabled && columns.Count(d => !d.FilterValue.IsEmpty()) > 0)
                writer.AppendHtmlLine(RenderFilterRowFooter(columns, _filter, _colCount));

            if (option.PagerEnabled && (option.PagerLocation == PagerLocationTypes.Bottom || option.PagerLocation == PagerLocationTypes.TopAndBottom))
            {
                if (data is PagedList<T> || data is StaticPagedList<T>)
                {
                    var pager = RenderPager(helper, data, option, currentPageSize, _queryItems);
                    writer.AppendHtmlLine(pager);
                }
                else
                {
                    var pager = RenderPager(helper, items, option, currentPageSize, _queryItems);
                    writer.AppendHtmlLine(pager);
                }
            }

            return new HtmlString(writer.GetString());
        }

        private static string RenderHeaderBar2(DataGridOptions4 option)
        {
            var builder = new HtmlContentBuilder();
            var printHeader = option.HeaderButtons.Count > 0;
            if (!printHeader)
                return string.Empty;
            builder.AppendHtml($@"<div class='btnHeaderList'>
<div class='dropdown'>
    <button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown'>
    {Text.HeaderButtonText} <span class='caret'></span>
</button>
<div class='dropdown-menu'>");
            if (option.HeaderButtons.Count > 0)
            {
                foreach (var button in option.HeaderButtons)
                    builder.AppendHtml($"{button.Value}");
            }
            builder.AppendHtml("</div></div></div>");
            return builder.GetString();
        }

        private static string RenderHeader<T>(IHtmlHelper helper, IEnumerable<DataColumn> columns, DataGridOptions4 option, Type firstItemType,
            string refreshUrl, Sorter sorter, Filter4 filter)
        {
            var builder = new HtmlContentBuilder();
            builder.AppendHtml("<tr>");
            var urlOperator = option.PagerAndShortAction.IndexOf("?") > -1 ? "&" : "?";
            if (option.ButtonShowEnabled || option.ButtonDeleteEnabled || option.ButtonEditEnabled || option.Buttons.Count > 0 || option.ButtonRefreshEnabled)
            {
                var width = string.Empty;
                if (option.ButtonColumnWidth > 0)
                    width = " width='" + option.ButtonColumnWidth + "'";
                builder.AppendHtml($"<th{width}>");
                builder.AppendHtml("<div class='btn-group'>");
                if (option.ButtonInsertEnabled)
                {
                    var action = option.ButtonInsertAction;
                    if (option.GridDiv != "divList")
                    {
                        if (action.Contains("?"))
                            action += "&";
                        else
                            action += "?";
                        action += "grid=" + option.GridDiv;
                    }
                    var buttonClass = option.InsertPopup ? option.ButtonInsertPopupCss : option.ButtonInsertCss;
                    var pop = option.InsertPopup ? option.Popup : "";
                    builder.AppendHtml(string.Format("<a href='{0}/{1}' class='{2}' {4}>{3}</a>",
                        $"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}",
                        action,
                        buttonClass,
                        "<i class=\"fas fa-plus\"></i>",
                        pop));
                }
                if (option.ButtonRefreshEnabled)
                    builder.AppendHtml($"<a onClick='{refreshUrl}' id='btnGridRefresh-{option.GridDiv}' class='{option.ButtonRefreshCss}'><i class=\"fas fa-sync-alt\"></i></a>");
                builder.AppendHtml("</div>");
                builder.AppendHtml("</th>");
            }
            foreach (var field in columns.Where(d => d.Visible && d.DataColumnType == DataColumnTypes.CommandColumn))
            {
                var cssClass = string.Empty;
                if (!field.HeaderCssClass.IsEmpty())
                    cssClass = $" class='{field.HeaderCssClass}'";
                var width = string.Empty;
                if (field.Width > 0)
                    width = $" width='{field.Width}'";
                if (field.Width > 0)
                    builder.AppendHtml($"<th{width}{cssClass}>");
                else
                    builder.AppendHtml($"<th{cssClass}>");
                builder.AppendHtml($"<span class='textHeader'>{field.Caption}</span>");
                builder.AppendHtml("</th>");
            }
            foreach (var field in columns.Where(d => d.Visible && d.DataColumnType != DataColumnTypes.CommandColumn))
            {
                var cssClass = string.Empty;
                if (!field.HeaderCssClass.IsEmpty())
                    cssClass = $" class='{field.HeaderCssClass}'";
                var width = string.Empty;
                if (field.Width > 0)
                    width = $" width='{field.Width}'";
                if (field.Width > 0)
                    builder.AppendHtml($"<th{width}{cssClass}>");
                else
                    builder.AppendHtml($"<th{cssClass}>");
                if (field.DataColumnType == DataColumnTypes.BoundColumn)
                {
                    if (field.Caption.IsEmpty())
                        field.Caption = Digger.GetDisplayName(typeof(T), field.FieldName);
                    builder.AppendHtml("<div class='tableDiv'>");
                    if (option.Sortable && field.Sortable)
                    {
                        var sortFieldName = field.SortField.IsEmpty() ? field.FieldName : field.SortField;
                        var sortImg = sorter.GetSortImg4(sortFieldName);
                        var sortParam = sorter.GetSortParam(sortFieldName);

                        var url = $@"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}/{option.PagerAndShortAction}{urlOperator}{option.SortKeyword}={sortParam}&{sorter.CleanQueryString}";

                        builder.AppendHtml($"<a class='textHeader' href=\"javascript:{option.PagerJsFunction}('{url}', '{option.GridDiv}');\">{field.Caption}</a>");
                        builder.AppendHtml(sortImg);
                    }
                    else
                        builder.AppendHtml($"<span class='textHeader'>{field.Caption}</span>");

                    if (option.FilteringEnabled && field.Filterable && (!field.FieldName.IsEmpty() || !field.FilterField.IsEmpty()))
                    {
                        var fieldType = field.FilterField.IsEmpty()
                                       ? Digger.GetType(firstItemType, field.FieldName)
                                       : Digger.GetType(firstItemType, field.FilterField);
                        if (filter.CanFilterable(fieldType))
                        {
                            var fReplace = field.FilterField.IsEmpty()
                                               ? field.FieldName.Replace(".", "__")
                                               : field.FilterField.Replace(".", "__");
                            builder.AppendHtml(string.Format("<a href='#filter_{0}' data-toggle='modal' class='textHeader filtre-image'><img class='tableImg' src='" +
                                    @"/assets/zero/mvcgrid/images/filter.png"
                                    + "' /></a>", fReplace));
                        }
                    }
                    builder.AppendHtml("</div>");
                }
                else if (field.DataColumnType == DataColumnTypes.SelectColumn)
                    builder.AppendHtml("<input type='checkbox' id='chkGridSelectAllRow' />");
                else
                    builder.AppendHtml($"<span class='textHeader'>{field.Caption}</span>");
                builder.AppendHtml("</th>");
            }
            builder.AppendHtml("</tr>");
            return builder.GetString();
        }

        private static string RenderFilterRow(IHtmlHelper helper, IEnumerable<DataColumn> columns, Type firstItemType, Filter4 filter)
        {
            var builder = new HtmlContentBuilder();
            foreach (var field in columns.Where(d => d.Visible))
            {
                if (!string.IsNullOrWhiteSpace(field.FieldName) || !string.IsNullOrWhiteSpace(field.FilterField))
                    builder.AppendHtml(filter.GetFilterDiv(field, firstItemType));
            }
            return builder.GetString();
        }

        private static string RenderRowButtons(IHtmlHelper helper, object item, DataGridOptions4 option)
        {
            var builder = new HtmlContentBuilder();
            if (!option.ButtonShowEnabled && !option.ButtonRefreshEnabled && !option.ButtonDeleteEnabled && !option.ButtonEditEnabled && option.Buttons.Count <= 0)
                return "";
            builder.AppendHtml("<td>");
            builder.AppendHtml("<div class='btn-group'>");

            var keyFieldValue = ValueConverter.GetFieldValue(item, option.KeyField);
            var keyText = ValueConverter.GetFieldValue(item, option.TextField);
            foreach (var button in option.Buttons)
            {
                builder.AppendHtml(button.GetString()
                                    .Replace("KEYFIELD", keyFieldValue)
                                    .Replace("KEYTEXT", keyText)
                                    .Replace("btn-mini", "btn-xs"));
            }
            if (option.RowOtherButtons.Count > 0)
            {
                builder.AppendHtml(@"<div class=""btn-group""><a class=""btn btn-xs dropdown-toggle text-wrench"" data-toggle=""dropdown"" href=""#"">" +
                   "<i class=\"fa fa-wrench\"></i> "
                    + @"</a><ul class=""dropdown-menu"">");
                foreach (var button in option.RowOtherButtons)
                    builder.AppendHtml("<li>" + button.GetString().Replace("KEYFIELD", keyFieldValue).Replace("KEYTEXT", keyText) + "</li>");
                builder.AppendHtml("</ul></div>");
            }
            if (option.ButtonShowEnabled)
            {
                var buttonClass = option.ButtonShowCss;
                var url = option.ButtonShowAction.IsEmpty()
                        ? $@"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}"
                        : $@"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}/{option.ButtonShowAction}";
                if (option.ButtonShowAction.Contains("?"))
                    url += $"&{option.KeyField}={keyFieldValue}";
                else
                    url += $"/{keyFieldValue}";

                var action = url;
                if (option.GridDiv != "divList")
                {
                    if (action.Contains("?"))
                        action += "&";
                    else
                        action += "?";
                    action += "grid=" + option.GridDiv;
                }
                builder.AppendHtml($"<a href='{action}' class='{buttonClass}'><i class='fas fa-search'></i></a>");
            }
            if (option.ButtonEditEnabled)
            {
                var drawEditButton = false;
                if (option.RowFormatClass != null && !option.ButtonEditShowFunction.IsEmpty())
                {
                    var obj = Activator.CreateInstance(option.RowFormatClass);
                    var a = option.RowFormatClass.GetMethod(option.ButtonEditShowFunction);
                    drawEditButton = (bool)a.Invoke(obj, new dynamic[1] { item });
                }
                else
                    drawEditButton = true;
                if (drawEditButton)
                {
                    var buttonClass = option.EditPopup ? option.ButtonEditPopupCss : option.ButtonEditCss;
                    var pop = option.EditPopup ? option.Popup : "";
                    var url = $@"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}/{option.ButtonEditAction}";
                    if (option.ButtonEditAction.Contains("?"))
                        url += $"&{option.KeyField}={keyFieldValue}";
                    else
                        url += $"/{keyFieldValue}";

                    var action = url;
                    if (option.GridDiv != "divList")
                    {
                        if (action.Contains("?"))
                            action += "&";
                        else
                            action += "?";
                        action += "grid=" + option.GridDiv;
                    }
                    var popIframe = option.EditPopup ? "data-fancybox-type='iframe'" : "";
                    builder.AppendHtml($"<a href='{action}' class='{buttonClass}' {pop} {popIframe}><i class='fas fa-edit'></i></a>");
                }
            }
            if (option.ButtonDeleteEnabled)
            {
                var drawDeleteButton = false;
                if (option.RowFormatClass != null && !option.ButtonDeleteShowFunction.IsEmpty())
                {
                    var obj = Activator.CreateInstance(option.RowFormatClass);
                    var a = option.RowFormatClass.GetMethod(option.ButtonDeleteShowFunction);
                    if (a == null)
                        throw new Exception("Geçersiz func: " + option.ButtonDeleteShowFunction);
                    drawDeleteButton = (bool)a.Invoke(obj, new dynamic[1] { item });
                }
                else
                    drawDeleteButton = true;
                if (drawDeleteButton)
                {
                    var url = $@"/{LibGeneral.GetContentUrl(helper.ViewContext.RouteData)}/{option.ButtonDeleteAction}";
                    if (option.ButtonEditAction.Contains("?"))
                        url += $"&{option.KeyField}={keyFieldValue}";
                    else
                        url += $"/{keyFieldValue}";

                    var action = url;
                    if (action.Contains("?"))
                        action += "&";
                    else
                        action += "?";
                    action += "grid=" + option.GridDiv;
                    var text = string.Format(Text.AskDelete, keyText).Replace("'", "\"");
                    builder.AppendHtml(string.Format(
                        "<a href='javascript:;' data-posturl='{0}' class='btnGridDelete {1}' data-confirm='{2}'><i class='fas fa-trash'></i></a>",
                        action,
                        option.ButtonDeleteCss,
                        text));
                }
            }
            builder.AppendHtml("</div>");
            builder.AppendHtml("</td>");
            return builder.GetString();
        }

        private static string RenderRowCommandButtons(IEnumerable<DataGridCommand> commands)
        {
            var builder = new HtmlContentBuilder();
            if (commands == null || commands.Count() == 0)
            {
                builder.AppendHtml("<td></td>");
                return builder.GetString();
            }
            builder.AppendHtml("<td class='btn-group'>");
            foreach (var command in commands)
            {
                if (command.Ajax)
                {
                    builder.AppendHtml($@"<a href='javascript:;' class='btnGridCommand {command.Css}' 
data-posturl='{command.Url}' data-confirm='{command.Confirm}'>{command.Title}</a>");
                }
                else
                    builder.AppendHtml($"<a href='{command.Url}' class='{command.Css}'>{command.Title}</a>");
            }
            builder.AppendHtml("</td>");
            return builder.GetString();
        }

        private static string RenderRow(IHtmlHelper helper, DataColumn[] columns, object item, DataGridOptions4 option)
        {
            var builder = new HtmlContentBuilder();
            var keyFieldValue = ValueConverter.GetFieldValue(item, option.KeyField);
            var idx = $" id='tr-{keyFieldValue}'";
            var idclass = "";
            if (option.RowFormatClass != null && !option.RowFormatFunction.IsEmpty())
            {
                var obj = Activator.CreateInstance(option.RowFormatClass);
                var a = option.RowFormatClass.GetMethod(option.RowFormatFunction);
                var sonuc = (string)a.Invoke(obj, new dynamic[1] { item });
                idclass = $" class='{sonuc}'";
            }
            builder.AppendHtml($"<tr{idx}{idclass}>");
            builder.AppendHtml(RenderRowButtons(helper, item, option));
            foreach (var column in columns.Where(d => d.Visible && d.DataColumnType == DataColumnTypes.CommandColumn))
            {
                var val = Digger.GetObjectValue(item, column.FieldName);
                if (val is List<DataGridCommand>)
                    builder.AppendHtml(RenderRowCommandButtons(val as List<DataGridCommand>));
            }
            foreach (var field in columns.Where(d => d.Visible && d.DataColumnType != DataColumnTypes.CommandColumn))
            {
                var attr = "";
                if (field.Width > 0)
                    attr += $" width='{field.Width}'";
                if (!field.CellCssClass.IsEmpty())
                    attr += $" class='{field.CellCssClass}'";
                builder.AppendHtml($"<td{attr}>");

                if (field.ObjectValueFunction.IsEmpty() || field.ObjectValueConverter == null)
                    builder.AppendHtml(ValueConverter.GetValue(helper, item, field));
                else
                {
                    var type = field.ObjectValueConverter;
                    var obj = Activator.CreateInstance(type);
                    var a = type.GetMethod(field.ObjectValueFunction);
                    if (a != null)
                    {
                        var sonuc = (string)a.Invoke(obj, new object[2] { helper, item });
                        builder.AppendHtml(sonuc);
                    }
                }
                builder.AppendHtml("</td>");
            }
            builder.AppendHtml("</tr>");
            return builder.GetString();
        }

        private static string RenderPager<T>(IHtmlHelper helper, IEnumerable<T> items, DataGridOptions4 option, int currentPageSize,
            List<KeyValuePair<string, string>> queryItems)
            where T : class
        {
            var pagedList = (IPagedList<T>)items;
            if (pagedList != null)
            {
                var urlOperator = option.PagerAndShortAction.IndexOf("?") > -1 ? "&" : "?";
                var qc = new List<KeyValuePair<string, string>>(queryItems);
                qc.RemoveAll(d => d.Key == option.PagerKeyword);
                qc.RemoveAll(d => d.Key == option.PageSizeKeyword);

                var qb = new QueryBuilder(qc);
                var pageSizeQs = qb.ToQueryString();
                var pgeIndexli = pageSizeQs.ToUriComponent();

                var ci = option.PagerAndShortAction.Split('?');
                if (ci.Count() > 1)
                    pgeIndexli = pgeIndexli.Replace(ci[1], "");
                if (pgeIndexli == "?")
                    pgeIndexli = "";
                if (pgeIndexli.Length > 1)
                    pgeIndexli = "&" + pgeIndexli.Substring(1);
                pgeIndexli = pgeIndexli.Replace("&&", "&");

                var pageUrl = string.Format(@"/{0}/{1}{6}{2}=[PAGE]&{4}={5}{3}",
                        LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                        option.PagerAndShortAction,
                        option.PagerKeyword,
                        pgeIndexli,
                        option.PageSizeKeyword,
                        currentPageSize,
                        urlOperator).Replace("[PAGE]", "{0}");

                var fs = string.Format(@"/{0}/{1}{4}{2}&{3}=XXX",
                            LibGeneral.GetContentUrl(helper.ViewContext.RouteData),
                            option.PagerAndShortAction,
                            pgeIndexli,
                            option.PageSizeKeyword,
                            urlOperator);
                fs = fs.Replace("?&", "?");
                fs = fs.Replace("&&", "&");
                fs = fs.Replace("??", "?");
                var pageSizeUrl = $"{option.PagerJsFunction}('{fs}', '{option.GridDiv}');";

                return HtmlHelper4.PagedListPager2(
                     helper,
                     pagedList,
                     page => $"javascript:{option.PagerJsFunction}('{string.Format(pageUrl, page)}','{option.GridDiv}');",
                     new PagedListRenderOptions
                     {
                         DisplayItemSliceAndTotal = true
                     },
                     currentPageSize,
                     pageSizeUrl,
                     option.PageSizeList);
            }
            return string.Empty;
        }

        private static string RenderFilterRowFooter(IEnumerable<DataColumn> columns, Filter4 filter, int colCount)
        {
            return filter.ActiveFilter(colCount, columns.ToList());
        }
    }
}