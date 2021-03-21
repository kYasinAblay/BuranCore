using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Data.Attributes;
using Buran.Core.MvcLibrary.Reflection;
using Buran.Core.MvcLibrary.Resource;
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
    [HtmlTargetElement("combobox2", Attributes = "brn-field")]
    public class ComboBox2TagHelper : TagHelper
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
        /// brn-data-url ile belirtilen adreski bilgileri alıp brn-subcombo-id elemanına doldurur.
        /// </summary>
        [HtmlAttributeName("brn-data-url")]
        public string DataUrl { get; set; }

        /// <summary>
        /// brn-data-url ile belirtilen adreski bilgileri alıp brn-subcombo-id elemanına doldurur.
        /// </summary>
        [HtmlAttributeName("brn-subcombo-id")]
        public string SubComboId { get; set; }

        /// <summary>
        /// SEÇİNİZ kelimesi
        /// </summary>
        [HtmlAttributeName("brn-option-label")]
        public string OptionLabel { get; set; }

        /// <summary>
        /// select bilgisine css class eklemek için kullanılır
        /// </summary>
        [HtmlAttributeName("brn-cssclass")]
        public string CssClass { get; set; }

        /// <summary>
        /// true ise bootstrap template olmadan sadece select nesnesi şeklinde gösterilir
        /// </summary>
        [HtmlAttributeName("brn-template")]
        public bool DisableEditorTemplate { get; set; }

        /// <summary>
        /// Template kapalı iken col-sm-X eklemeyi kapatır
        /// </summary>
        [HtmlAttributeName("brn-colsize")]
        public bool DisableColSize { get; set; }

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

        public ComboBox2TagHelper(IHtmlHelper htmlHelper, IServiceProvider provider)
        {
            LabelColCount = 3;
            EditorColCount = 9;
            ReadOnly = false;
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
                        if (a.GetParameters().Length == 1)
                        {
                            var dataList = a.Invoke(obj, new object[1] { metadata.Model });
                            result.ListItems = dataList as SelectList;
                        }
                        else if (a.GetParameters().Length == 2)
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

            var htmlId = prefix.IsEmpty()
                ? ModelItem.Metadata.PropertyName
                : _htmlHelper.IdForModel() != ModelItem.Metadata.PropertyName
                    ? _htmlHelper.IdForModel() + "_" + ModelItem.Metadata.PropertyName
                    : ModelItem.Metadata.PropertyName;

            var htmlName = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : prefix + "." + ModelItem.Metadata.PropertyName;
            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            if (ItemList == null)
            {
                var comboDataInfo = GetComboBoxDataSource(ModelItem.ModelExplorer);
                ItemList = comboDataInfo.ListItems;
                if (comboDataInfo.CanSelect)
                {
                    if (OptionLabel.IsEmpty())
                        OptionLabel = UI.Select;
                }
                else
                    OptionLabel = null;
            }
            var sbOptions = new StringBuilder();
            if (!OptionLabel.IsEmpty())
                sbOptions.AppendLine(string.Concat("<option value=\"\">", OptionLabel, "</option>"));
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

            if (DisableEditorTemplate)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("id", "div" + htmlId);
                if (!DisableColSize)
                    output.Attributes.Add("class", $"col-sm-{EditorColCount}");

                var select = new TagBuilder("select");
                if (ReadOnly)
                    select.Attributes.Add("disabled", "disabled");
                if (!DataUrl.IsEmpty())
                {
                    select.Attributes.Add("onChange", $"LoadDropdown('{DataUrl}','{htmlId}', '{(SubComboId.IsEmpty() ? "" : SubComboId)}', '{(OptionLabel.IsEmpty() ? "" : OptionLabel)}');");
                }
                select.Attributes.Add("name", htmlName);
                select.AddCssClass("form-control form-control-sm");
                if (!CssClass.IsEmpty())
                    select.AddCssClass(CssClass);
                select.Attributes.Add("id", htmlId);
                if (ModelItem.Metadata.IsRequired)
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
                output.Content.SetHtmlContent(select.GetString());
            }
            else
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "form-group row");
                output.Attributes.Add("id", "div" + htmlId);

                var select = new TagBuilder("select");
                if (ReadOnly)
                    select.Attributes.Add("disabled", "disabled");
                if (!DataUrl.IsEmpty())
                {
                    select.Attributes.Add("onChange", $"LoadDropdown('{DataUrl}','{htmlId}', '{(SubComboId.IsEmpty() ? "" : SubComboId)}', '{(OptionLabel.IsEmpty() ? "" : OptionLabel)}');");
                }
                select.AddCssClass("form-control form-control-sm");
                if (!CssClass.IsEmpty())
                    select.AddCssClass(CssClass);
                select.Attributes.Add("name", htmlName);
                select.Attributes.Add("id", htmlId);

                var irq = (!IsRequired.HasValue && ModelItem.Metadata.GetIsRequired()) || (IsRequired.HasValue && IsRequired.Value);
                var requiredHtml = irq
                    ? $"<span class=\"{RequiredCssClass}\">{Symbol}</span>"
                    : "";
                var metaHtml = irq
                    ? $"<span class=\"field-validation-valid help-block\" data-valmsg-for=\"{htmlId}\" data-valmsg-replace=\"true\"></span>"
                    : "";

                if (irq)
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

                var addHtml = "";
                if (!AddNewUrl.IsEmpty())
                {
                    addHtml = $@"<div class='col-1'>
                <a href='{AddNewUrl}' class='btn btn-sm btn-primary fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
            </div>";
                    EditorColCount--;
                }

                var editorTemplate = $@"
                    <label class=""col-{LabelColCount} col-form-label col-form-label-sm"">{labelText} {requiredHtml}</label>
                    <div class=""col-{EditorColCount}"">
                        {select.GetString()}
                        {metaHtml}
                    </div>
                    {addHtml}";
                output.Content.SetHtmlContent(editorTemplate);

            }
        }
    }
}
