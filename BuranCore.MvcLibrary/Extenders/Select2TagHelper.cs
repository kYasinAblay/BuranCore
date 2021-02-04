using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("select2")]
    public class Select2TagHelper : TagHelper
    {
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        [HtmlAttributeName("brn-fieldname")]
        public string FieldName { get; set; }

        [HtmlAttributeName("brn-labeltext")]
        public string LabelText { get; set; }

        [HtmlAttributeName("brn-itemlist")]
        public List<TokenInputValue> ItemList { get; set; }


        [HtmlAttributeName("brn-url")]
        public string Url { get; set; }

        [HtmlAttributeName("brn-loader-url")]
        public string LoaderUrl { get; set; }


        [HtmlAttributeName("brn-cssclass")]
        public string CssClass { get; set; }


        [HtmlAttributeName("brn-placeholder")]
        public string PlaceHolderText { get; set; }

        [HtmlAttributeName("brn-multi-select")]
        public bool MultiSelect { get; set; }

        [HtmlAttributeName("brn-can-clear-select")]
        public bool CanClearSelect { get; set; }

        [HtmlAttributeName("brn-parent-combobox")]
        public string ParentComboBox { get; set; }

        [HtmlAttributeName("brn-disable-editor-template")]
        public bool DisableEditorTemplate { get; set; }

        [HtmlAttributeName("brn-disable-form-group")]
        public bool DisableFormGroup { get; set; }

        [HtmlAttributeName("brn-disable-js")]
        public bool DisableJs { get; set; }


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
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Select2TagHelper(IHtmlHelper htmlHelper)
        {
            LabelColCount = 3;
            EditorColCount = 9;
            _htmlHelper = htmlHelper;

            RequiredCssClass = "editor-field-required";
            Symbol = " *";
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            var prefix = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;

            var htmlId = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : _htmlHelper.IdForModel() + "_" + ModelItem.Metadata.PropertyName;
            var htmlName = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : prefix + "." + ModelItem.Metadata.PropertyName;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            if (!FieldName.IsEmpty())
            {
                htmlId = FieldName;
                htmlName = FieldName;
            }
            if (!LabelText.IsEmpty())
                labelText = LabelText;

            var initalJson = string.Empty;
            var selectionJs = string.Empty;
            if (ItemList != null && ItemList.Count > 0)
            {
                var json = "[";
                var fistItem = true;
                foreach (var value in ItemList)
                {
                    if (!fistItem)
                    {
                        json += ",";
                    }
                    json += "{id: " + value.Id + ", text: \"" + value.Name + "\"}";
                    fistItem = false;
                }
                json += "]";
                initalJson = string.Format("$('#{0}').select2('data', {1});", htmlId, json);
            }
            else
            {
                selectionJs = $@"initSelection: function(element, callback) {{
            var id = $(element).val();
            if (id != '') {{
                $.ajax('{LoaderUrl}/' + id, {{
                    dataType: 'json'
                }}).done(function(data) {{ callback(data); }});
            }}
        }}";
            }

            string js = DisableJs
                ? ""
                : $@"<script type=""text/javascript"">
$(function () {{
    $(""#{htmlId}"").select2({{
        placeholder: ""{PlaceHolderText}"",
        {(CanClearSelect ? "allowClear: true, " : "")}
        {(MultiSelect ? "multiple:true," : "")}
        minimumInputLength: 1,
        ajax: {{
            url: '{Url}',
            dataType: 'json',
            data: function (term, page) {{
                return {{
                    q: term,
                    {(!ParentComboBox.IsEmpty() ? $"id: $('#{ParentComboBox}').val()," : "")}
                }};
            }},
            results: function (data, page) {{
                return {{ results: data }};
            }}
        }},
        {selectionJs}
    }});
    {initalJson}
}});
</script>";

            var valHtml = ModelItem != null ? "value='" + ModelItem.Model + "'" : "";
            if (DisableEditorTemplate)
            {

                if (!AddNewUrl.IsEmpty())
                {
                    var sep = "?";
                    if (AddNewUrl.Contains("?"))
                        sep = "&";
                    AddNewUrl += sep + "editorId=" + htmlId;
                    output.Content.SetHtmlContent($@"
<div class=""input-group"">
    <input id='{htmlId}' name='{htmlName}' type='text' class='form-control input-sm {CssClass}' {valHtml} />
    <span class=""input-group-btn"">
        <a href='{AddNewUrl}' class='btn btn-xs btn-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fa fa-plus'></i></a>
    </span>
    {js}
</div>");
                }
                else
                {
                    output.Content.SetHtmlContent($@"
<input id='{htmlId}' name='{htmlName}' type='text' class='form-control input-sm {CssClass}' {valHtml} />
{js}");
                }
            }
            else
            {
                if (!DisableFormGroup)
                {
                    output.Attributes.Add("class", "form-group");
                    output.Attributes.Add("id", "div" + htmlId);
                }
                else
                {
                    output.TagName = "span";
                }

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
<label class=""col-xs-{LabelColCount} control-label"">{labelText} {requiredHtml}</label>
<div class=""col-xs-{EditorColCount}"">
    <div class=""input-group"">
        <input id='{htmlId}' name='{htmlName}' type='text' class='form-control input-sm {CssClass}' {valHtml} />
        <span class=""input-group-btn"">
            <a href='{AddNewUrl}' class='btn btn-xs btn-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fa fa-plus'></i></a>
        </span>
    </div>
    {metaHtml}
</div>
{js}");
                }
                else
                {
                    output.Content.SetHtmlContent($@"
<label class=""col-xs-{LabelColCount} control-label"">{labelText} {requiredHtml}</label>
<div class=""col-xs-{EditorColCount}"">
    <input id='{htmlId}' name='{htmlName}' type='text' class='form-control input-sm {CssClass}' {valHtml} />
    {metaHtml}
</div>
{js}");
                }
            }
        }
    }
}
