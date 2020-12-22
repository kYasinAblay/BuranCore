using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Grid;
using Buran.Core.MvcLibrary.Grid.Columns;
using Buran.Core.MvcLibrary.Grid.Helper;
using Buran.Core.MvcLibrary.Grid.Pager;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("brngrid")]
    public class DataGridTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-model")]
        public IEnumerable<dynamic> ModelData { get; set; }

        [HtmlAttributeName("brn-colums")]
        public List<DataColumn> Columns { get; set; }

        [HtmlAttributeName("brn-options")]
        public DataGridOptions Options { get; set; }


        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public DataGridTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }


        private Sorter _sorter;
        private Filter _filter;
        private string _refreshUrl = string.Empty;
        private int _colCount;
        private QueryString _query;
        private Dictionary<string, StringValues> _queryDictionary;
        private List<KeyValuePair<string, string>> _queryItems;
        private string _queryParams = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            var prefix = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;

            _query = _htmlHelper.ViewContext.HttpContext.Request.QueryString;
            _queryDictionary = QueryHelpers.ParseQuery(_query.ToString());
            _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            _queryParams = _query.ToUriComponent();

            if (!Options.PagerAndShortAction.IsEmpty())
            {
                var psss = Options.PagerAndShortAction.Split('?');
                if (psss.Length > 1)
                {
                    _queryParams = _queryParams.Replace("?" + psss[1], "");
                }
            }

            _colCount = 0;
            var items = ModelData;
            if (items == null)
            {
                output.Content.SetHtmlContent(Options.EmptyData);
                return;
            }

            _sorter = new Sorter(_queryItems, Options.SortKeyword, Options.PagerAndShortAction, _htmlHelper);
            _filter = new Filter(_queryDictionary, _queryItems, Options.PagerKeyword,
                _htmlHelper.ViewContext.RouteData, Options.PagerAndShortAction,
                _htmlHelper, Options.PagerJsFunction, Options.GridDiv);

            if (Options.FilteringEnabled) // && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var kp = _filter.GetWhereString();
                if (kp.Where != null)
                    items = items.AsQueryable().Where(kp.Where, kp.Params.ToArray());
            }

            #region SORT DATA
            if (Options.Sortable) // && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                if (_sorter.List.Count == 0 && Options.DefaultSorting)
                {
                    if (!Options.SortDefaultFieldName.IsEmpty())
                        _sorter.List.Add(new SorterInfo() { Direction = Options.SortDefaultDirection, Keyword = Options.SortDefaultFieldName });
                    if (Options.SortDefaultFieldNames != null)
                    {
                        foreach (string sortinfodefaultname in Options.SortDefaultFieldNames)
                        {
                            _sorter.List.Add(new SorterInfo()
                            {
                                Direction = Options.SortDefaultDirection,
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
            var currentPageSize = Options.DefaultPageSize;
            var currentPageIndexItem = _queryItems.FirstOrDefault(d => d.Key == Options.PagerKeyword);
            var pageIndex = 0;
            if (!currentPageIndexItem.Value.IsEmpty())
            {
                int.TryParse(currentPageIndexItem.Value, out pageIndex);
            }
            if (Options.PagerEnabled) // && !(data is PagedList<T>) && !(data is StaticPagedList<T>))
            {
                var currentPageSizeStr = _queryItems.FirstOrDefault(d => d.Key == Options.PageSizeKeyword);
                if (!currentPageSizeStr.Value.IsEmpty())
                {
                    int.TryParse(currentPageSizeStr.Value, out currentPageSize);
                }

                items = new PagedList<dynamic>(items, pageIndex, currentPageSize);
                //TotalRowCount = (items as PagedList<T>).TotalItemCount;
                //TotalPageCount = (items as PagedList<T>).PageCount;
            }
            #endregion


            var T = ModelData.GetType();
            //T firstItem = default(T);
            //if (items != null && items.Any())
            //    firstItem = items.First();
            //Type firstItemType = null;
            //if (firstItem != null)
            //    firstItemType = firstItem.GetType();

            var writer = new StringBuilder();
            _refreshUrl = GetRefreshUrl(_htmlHelper, Options);

            if (Options.PagerEnabled && (Options.PagerLocation == PagerLocationTypes.Top || Options.PagerLocation == PagerLocationTypes.TopAndBottom))
            {
                //if (data is PagedList<T> || data is StaticPagedList<T>)
                //{
                //    var pager = RenderPager(helper, data, option, currentPageSize);
                //    writer.AppendHtmlLine(pager);
                //}
                //else
                //{
                //var pager = RenderPager(_htmlHelper, items, Options, currentPageSize);
                //writer.AppendHtmlLine(pager);
                //}
            }



            //            output.TagName = "div";
            //            output.TagMode = TagMode.StartTagAndEndTag;
            //            if (Width > 0)
            //                output.Attributes.Add("style", $"width:{Width}px");

            //            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            //            var selectionJs = string.Empty;
            //            if (SelectedValues != null && SelectedValues.Count > 0)
            //            {
            //                var sb = new StringBuilder();
            //                foreach (var s in SelectedValues)
            //                {
            //                    sb.AppendLine($@"$('#{htmlId}').append(new Option('{s.Name}', {s.Id}, true, true));");
            //                }
            //                sb.AppendLine($@"$('#{htmlId}').trigger('change');");
            //                selectionJs = sb.ToString();
            //            }
            //            else
            //            {
            //                var valId = ModelItem != null && ModelItem.Model != null ? ModelItem.Model.ToString() : "";
            //                if (!valId.IsEmpty())
            //                {
            //                    selectionJs = $@"
            // $.ajax('{LoaderUrl}/{valId}', {{dataType: 'json'}}).done(function(data) {{ 
            //    var option = new Option(data.text, data.id, true, true);
            //    $(""#{htmlId}"").append(option).trigger('change');
            //}});";
            //                }
            //            }

            //            string js = DisableJs
            //                ? ""
            //                : $@"<script type=""text/javascript"">
            //$(function () {{
            //    $(""#{htmlId}"").select2({{
            //        placeholder: ""{PlaceHolderText}"",
            //        language: ""tr"",
            //        {(CanClearSelect ? "allowClear: true, " : "")}
            //        {(MultiSelect ? "multiple:true," : "")}
            //        minimumInputLength: 1,
            //        ajax: {{
            //            url: '{Url}',
            //            data: function (params) {{
            //                return {{
            //                    q: params.term,
            //                    {(!ParentComboBox.IsEmpty() ? $"id: $('#{ParentComboBox}').val()," : "")}
            //                }};
            //            }},
            //            processResults: function (data) {{
            //                return {{ results: data }};
            //            }}
            //        }},
            //    }});
            //    {selectionJs}
            //}});
            //</script>";

            //            var valHtml = ModelItem != null ? "value='" + ModelItem.Model + "'" : "";
            //            if (DisableEditorTemplate)
            //            {
            //                if (!DisableColSize)
            //                    output.Attributes.Add("class", $"col-sm-{EditorColCount}");
            //                output.Attributes.Add("id", "div" + htmlId);
            //                if (!AddNewUrl.IsEmpty())
            //                {
            //                    var sep = "?";
            //                    if (AddNewUrl.Contains("?"))
            //                        sep = "&";
            //                    AddNewUrl += sep + "editorId=" + htmlId;
            //                    output.Content.SetHtmlContent($@"
            //<div class=""input-group input-group-sm"">
            //    <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
            //    </select>
            //    <div class=""input-group-append"">
            //        <a href='{AddNewUrl}' class='btn btn-sm btn-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
            //    </div>
            //    {js}
            //</div>");
            //                }
            //                else
            //                {
            //                    output.Content.SetHtmlContent($@"
            //<select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
            //</select>
            //{js}");
            //                }
            //            }
            //            else
            //            {
            //                output.Attributes.Add("class", "form-group row");
            //                output.Attributes.Add("id", "div" + htmlId);
            //                var irq = (!IsRequired.HasValue && ModelItem.Metadata.GetIsRequired()) || (IsRequired.HasValue && IsRequired.Value);
            //                var requiredHtml = irq
            //                    ? $"<span class=\"{RequiredCssClass}\">{Symbol}</span>"
            //                    : "";
            //                var metaHtml = irq
            //                    ? $"<span class=\"field-validation-valid help-block\" data-valmsg-for=\"{htmlId}\" data-valmsg-replace=\"true\"></span>"
            //                    : "";
            //                if (!AddNewUrl.IsEmpty())
            //                {
            //                    var sep = "?";
            //                    if (AddNewUrl.Contains("?"))
            //                        sep = "&";
            //                    AddNewUrl += sep + "editorId=" + htmlId;
            //                    output.Content.SetHtmlContent($@"
            //<label class=""col-sm-{LabelColCount} col-form-label control-label-sm"">{labelText} {requiredHtml}</label>
            //<div class=""col-sm-{EditorColCount}"">
            //    <div class=""input-group input-group-sm"">
            //        <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
            //        </select>
            //        <div class=""input-group-append"">
            //            <a href='{AddNewUrl}' class='btn btn-sm btn-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
            //        </div>
            //    </div>
            //    {metaHtml}
            //</div>
            //{js}");
            //                }
            //                else
            //                {
            //                    output.Content.SetHtmlContent($@"
            //<label class=""col-sm-{LabelColCount} col-form-label col-form-label col-form-label-sm"">{labelText} {requiredHtml}</label>
            //<div class=""col-sm-{EditorColCount}"">
            //    <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml} >
            //    </select>
            //    {metaHtml}
            //</div>
            //{js}");


        }

        private string GetRefreshUrl(IHtmlHelper helper, DataGridOptions option)
        {
            var baseUrl = LibGeneral.GetContentUrl(helper.ViewContext.RouteData);
            var fs = $@"/{baseUrl}/{option.PagerAndShortAction}{_queryParams}";
            return $"{option.PagerJsFunction}(\"{fs}\",\"{option.GridDiv}\");";
        }
    }
}