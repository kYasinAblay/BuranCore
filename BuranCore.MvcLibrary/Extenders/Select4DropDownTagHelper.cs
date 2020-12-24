using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Data.Attributes;
using Buran.Core.MvcLibrary.Reflection;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("select4dropdown")]
    public class Select4DropDownTagHelper : TagHelper
    {
        /// <summary>
        /// Alan adı
        /// </summary>
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        /// <summary>
        /// SelectList tipinde gösterilecek liste elemanları
        /// </summary>
        [HtmlAttributeName("brn-items")]
        public SelectList ItemList { get; set; }

        /// <summary>
        /// List<SelectListItem> tipinde gösterilecek liste elemanları
        /// </summary>
        [HtmlAttributeName("brn-items2")]
        public List<SelectListItem> ItemList2 { get; set; }

        /// <summary>
        /// SEÇİNİZ bilgisi
        /// </summary>
        [HtmlAttributeName("brn-placeholder")]
        public string PlaceHolderText { get; set; }

        /// <summary>
        /// Çoklu seçim aktif/pasif. Varsayılan false
        /// </summary>
        [HtmlAttributeName("brn-multi-select")]
        public bool MultiSelect { get; set; }

        /// <summary>
        /// Tag girişini aktif eder.
        /// </summary>
        [HtmlAttributeName("brn-tag")]
        public bool Tag { get; set; }

        /// <summary>
        /// select bilgisine css class eklemek için kullanılır
        /// </summary>
        [HtmlAttributeName("brn-cssclass")]
        public string CssClass { get; set; }

        /// <summary>
        /// Seçimi temizle düğmesi aktif/pasif. Varsayılan false
        /// </summary>
        [HtmlAttributeName("brn-can-clear-select")]
        public bool CanClearSelect { get; set; }

        /// <summary>
        /// Edit modunda ise val change sonrası change event trigger etmez
        /// </summary>
        [HtmlAttributeName("brn-edit-mode")]
        public bool InEditMode { get; set; }

        /// <summary>
        /// true ise bootstrap template olmadan sadece select nesnesi şeklinde gösterilir
        /// </summary>
        [HtmlAttributeName("brn-template")]
        public bool DisableEditorTemplate { get; set; }

        /// <summary>
        /// select2 için oluşturulan script tagını render etmez. Varsayılan false
        /// </summary>
        [HtmlAttributeName("brn-disable-js")]
        public bool DisableJs { get; set; }

        /// <summary>
        /// Yeni ekleme düğmesi aktif olur ve ekleme url bilgi
        /// </summary>
        [HtmlAttributeName("brn-add-new-url")]
        public string AddNewUrl { get; set; }

        /// <summary>
        /// bootstrap template için label col büyüklüğü
        /// </summary>
        [HtmlAttributeName("brn-label-col")]
        public int LabelColCount { get; set; }

        /// <summary>
        /// bootstrap template için editor col büyüklüğü
        /// </summary>
        [HtmlAttributeName("brn-editor-col")]
        public int EditorColCount { get; set; }

        /// <summary>
        /// select tagı için width belirler
        /// </summary>
        [HtmlAttributeName("brn-width")]
        public int Width { get; set; }

        /// <summary>
        /// Label için zorunlu işaretini modelden okumayı iptal eder ve bu bilgiden belirler
        /// </summary>
        [HtmlAttributeName("brn-isrequired")]
        public bool? IsRequired { get; set; }

        /// <summary>
        /// zorunlu işareti için css class bilgisi
        /// </summary>
        [HtmlAttributeName("brn-required-css-class")]
        public string RequiredCssClass { get; set; }

        /// <summary>
        /// Zorunlu işateri
        /// </summary>
        [HtmlAttributeName("brn-symbol")]
        public string Symbol { get; set; }

        [HtmlAttributeName("brn-readonly")]
        public bool ReadOnly { get; set; }


        private IHtmlHelper _htmlHelper;
        private IServiceProvider _serviceProvider;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Select4DropDownTagHelper(IHtmlHelper htmlHelper, IServiceProvider provider)
        {
            LabelColCount = 3;
            EditorColCount = 9;
            _htmlHelper = htmlHelper;
            _serviceProvider = provider;

            RequiredCssClass = "editor-field-required";
            Symbol = " *";
        }

        private ComboBoxDataInfo GetComboBoxDataSource(ModelExplorer metadata)
        {
            ComboBoxDataInfo result = new ComboBoxDataInfo();
            var comboDataModel = Digger2.GetMetaAttr<ComboBoxDataAttribute>(metadata.Metadata);
            if (comboDataModel != null)
            {
                result.CanSelect = comboDataModel.ShowSelect;

                if (comboDataModel.Repository != null)
                {
                    var repo = comboDataModel.Repository;
                    if (repo != null)
                    {
                        var obj = ActivatorUtilities.CreateInstance(_serviceProvider, repo);
                        var a = repo.GetMethod(comboDataModel.QueryName);
                        if (a == null)
                            return null;
                        if (a.GetParameters().Count() == 1)
                        {
                            var dataList = a.Invoke(obj, new object[1] { metadata.Model });
                            result.ListItems = dataList as SelectList;
                        }
                        else if (a.GetParameters().Count() == 2)
                        {
                            var dataList = a.Invoke(obj, new object[2] { metadata.Model, false });
                            result.ListItems = dataList as SelectList;
                        }
                    }
                }
            }
            return result;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            var prefix = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (Width > 0)
                output.Attributes.Add("style", $"width:{Width}px");

            var htmlId = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : _htmlHelper.IdForModel() + "_" + ModelItem.Metadata.PropertyName;
            var htmlName = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : prefix + "." + ModelItem.Metadata.PropertyName;
            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            if (ItemList == null)
            {
                var comboDataInfo = GetComboBoxDataSource(ModelItem.ModelExplorer);
                ItemList = comboDataInfo.ListItems;
            }
            var sbOptions = new StringBuilder();
            if (CanClearSelect)
                sbOptions.AppendLine(string.Concat("<option value=\"\"></option>"));
            if (ItemList != null)
            {
                foreach (var item in ItemList)
                {
                    var option = new TagBuilder("option");
                    option.Attributes.Add("value", item.Value);
                    if (item.Selected)
                        option.Attributes.Add("selected", "selected");
                    option.InnerHtml.AppendHtml(item.Text);
                    sbOptions.AppendLine(option.GetString());
                }
            }
            else if (ItemList2 != null)
            {
                foreach (var item in ItemList2)
                {
                    var option = new TagBuilder("option");
                    option.Attributes.Add("value", item.Value);
                    if (item.Selected)
                        option.Attributes.Add("selected", "selected");
                    option.InnerHtml.AppendHtml(item.Text);
                    sbOptions.AppendLine(option.GetString());
                }
            }
            var select = new TagBuilder("select");
            select.AddCssClass("form-control form-control-sm");
            if (!CssClass.IsEmpty())
                select.AddCssClass(CssClass);
            select.Attributes.Add("name", htmlName);
            select.Attributes.Add("id", htmlId);
            if (MultiSelect)
                select.Attributes.Add("multiple", "multiple");

            if (!IsRequired.HasValue && ModelItem.Metadata.GetIsRequired())
            {
                var requiredAttr = Digger2.GetMetaAttr<RequiredAttribute>(ModelItem.Metadata);
                var errMsg = "";
                if (requiredAttr != null)
                {
                    errMsg = requiredAttr.ErrorMessage;
                    if (errMsg.IsEmpty() && requiredAttr.ErrorMessageResourceType != null)
                    {
                        var rm = new ResourceManager(requiredAttr.ErrorMessageResourceType);
                        var rsm = rm.GetString(requiredAttr.ErrorMessageResourceName);
                        if (rsm != null && !rsm.IsEmpty())
                            errMsg = string.Format(rsm, labelText);
                    }
                }
                select.Attributes.Add("data-val", "true");
                select.Attributes.Add("data-val-required",
                    requiredAttr != null
                        ? errMsg
                        : "Gereklidir"
                );
            }
            select.InnerHtml.AppendHtml(sbOptions.ToString());

            string js = DisableJs
                ? ""
                : $@"
<script type=""text/javascript"">
$(function () {{
    $(""#{htmlId}"").select2({{
        language: ""tr"",
        placeholder: ""{(PlaceHolderText.IsEmpty() ? "" : PlaceHolderText)}"",
        {(MultiSelect ? "multiple:true," : "")}
        {(Tag ? "tags:true," : "")}
        {(ReadOnly ? "disabled:true," : "")}
        {(CanClearSelect ? "allowClear: true" : "")}
    }});
}});
</script>";

            if (DisableEditorTemplate)
            {
                output.Attributes.Add("id", "div" + htmlId);
                output.Attributes.Add("class", $"col-sm-{EditorColCount}");

                if (!AddNewUrl.IsEmpty())
                {
                    var sep = "?";
                    if (AddNewUrl.Contains("?"))
                        sep = "&";
                    AddNewUrl += sep + "editorId=" + htmlId;
                    output.Content.SetHtmlContent($@"
<div class=""input-group input-group-sm"">
    {select.GetString()}
    <div class=""input-group-append"">
        <a href='{AddNewUrl}' class='btn btn-sm btn-default btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
    </div>
    {js}
</div>");
                }
                else
                {
                    output.Content.SetHtmlContent($@"{select.GetString()}
{js}");
                }
            }
            else
            {
                output.Attributes.Add("class", "form-group row");
                output.Attributes.Add("id", "div" + htmlId);
                var irq = (!IsRequired.HasValue && ModelItem.Metadata.GetIsRequired()) || (IsRequired.HasValue && IsRequired.Value);
                var requiredHtml = irq
                    ? $"<span class=\"{RequiredCssClass}\">{Symbol}</span>"
                    : "";
                var metaHtml = irq
                    ? $"<span class=\"field-validation-valid help-block\" data-valmsg-for=\"{htmlId}\" data-valmsg-replace=\"true\"></span>"
                    : "";
                if (!AddNewUrl.IsEmpty())
                {
                    var sep = "?";
                    if (AddNewUrl.Contains("?"))
                        sep = "&";
                    AddNewUrl += sep + "editorId=" + htmlId;
                    output.Content.SetHtmlContent($@"
<label class=""col-sm-{LabelColCount} col-form-label col-form-label-sm"">{labelText} {requiredHtml}</label>
<div class=""col-sm-{EditorColCount}"">
    <div class=""input-group input-group-sm"">
        {select.GetString()}
        <div class=""input-group-append"">
            <a href='{AddNewUrl}' class='btn btn-sm btn-default btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
        </div>
    </div>
    {metaHtml}
</div>
{js}");
                }
                else
                {
                    output.Content.SetHtmlContent($@"
<label class=""col-sm-{LabelColCount} col-form-label col-form-label-sm"">{labelText} {requiredHtml}</label>
<div class=""col-sm-{EditorColCount}"">
    {select.GetString()}
    {metaHtml}
</div>
{js}");
                }
            }
        }
    }
}
