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
    [Obsolete]
    [HtmlTargetElement("select", Attributes = "brn-field")]
    public class ComboBoxTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        [HtmlAttributeName("brn-items")]
        public SelectList ItemList { get; set; }

        [HtmlAttributeName("brn-items2")]
        public List<SelectListItem> ItemList2 { get; set; }

        [HtmlAttributeName("brn-data-url")]
        public string DataUrl { get; set; }

        [HtmlAttributeName("brn-option-label")]
        public string OptionLabel { get; set; }



        [HtmlAttributeName("brn-disable-editor-template")]
        public bool DisableEditorTemplate { get; set; }

        [HtmlAttributeName("brn-add-new-url")]
        public string AddNewUrl { get; set; }

        [HtmlAttributeName("brn-label-col")]
        public int LabelColCount { get; set; }
        [HtmlAttributeName("brn-editor-col")]
        public int EditorColCount { get; set; }



        [HtmlAttributeName("brn-isrequired")]
        public bool? IsRequired { get; set; }

        [HtmlAttributeName("brn-required-css-class")]
        public string RequiredCssClass { get; set; }

        [HtmlAttributeName("brn-symbol")]
        public string Symbol { get; set; }




        private IHtmlHelper _htmlHelper;
        private IServiceProvider _serviceProvider;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        
        public ComboBoxTagHelper(IHtmlHelper htmlHelper, IServiceProvider provider)
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
                        var obj = Activator.CreateInstance(repo);
                        //var obj = ActivatorUtilities.CreateInstance(_serviceProvider, repo);
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
                output.TagName = "select";
                output.TagMode = TagMode.StartTagAndEndTag;
                if (!DataUrl.IsEmpty())
                    output.Attributes.Add("data-url", DataUrl);
                output.Attributes.Add("name", htmlName);
                output.Attributes.Add("id", htmlId);
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
                    output.Attributes.Add("data-val", "true");
                    output.Attributes.Add("data-val-required",
                        requiredAttr != null
                            ? errMsg
                            : "Gereklidir"
                    );
                }
                output.Content.SetHtmlContent(sbOptions.ToString());
            }
            else
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "form-group");
                output.Attributes.Add("id", "div" + htmlId);

                var select = new TagBuilder("select");
                if (!DataUrl.IsEmpty())
                    select.Attributes.Add("data-url", DataUrl);
                select.AddCssClass("form-control input-sm");
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
                    addHtml = $@"<div class='col-xs-1'>
                <a href='{AddNewUrl}' class='btn btn-xs btn-primary fancyboxAdd fancybox.iframe'><i class='fa fa-plus'></i></a>
            </div>";
                    EditorColCount--;
                }

                var editorTemplate = $@"
                    <label class=""col-xs-{LabelColCount} control-label"">{labelText} {requiredHtml}</label>
                    <div class=""col-xs-{EditorColCount}"">
                        {select.GetString()}
                        {metaHtml}
                    </div>
                    {addHtml}";
                output.Content.SetHtmlContent(editorTemplate);

            }
        }
    }
}
