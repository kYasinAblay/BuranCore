using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("select4")]
    public class Select4TagHelper : TagHelper
    {
        /// <summary>
        /// Alan adı
        /// </summary>
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        ///// <summary>
        ///// Model bind etmeden, alan adını manuel belirler
        ///// </summary>
        //[HtmlAttributeName("brn-fieldname")]
        //public string FieldName { get; set; }

        ///// <summary>
        ///// FieldName kullanıldığı zaman label bilgisini belirler
        ///// </summary>
        //[HtmlAttributeName("brn-labeltext")]
        //public string LabelText { get; set; }

        /// <summary>
        /// MultiSelect aktif iken seçili gelecek bilgiler
        /// </summary>
        [HtmlAttributeName("brn-selecedList")]
        public List<TokenInputValue> SelectedValues { get; set; }

        /// <summary>
        /// Liste elemanlarının getirileceği ajax url bilgisi
        /// </summary>
        [HtmlAttributeName("brn-url")]
        public string Url { get; set; }

        /// <summary>
        /// Eleman seçimi sonrası çalışarak text bilgisini yazar
        /// </summary>
        [HtmlAttributeName("brn-loader-url")]
        public string LoaderUrl { get; set; }

        /// <summary>
        /// select bilgisine css class eklemek için kullanılır
        /// </summary>
        [HtmlAttributeName("brn-cssclass")]
        public string CssClass { get; set; }

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
        /// Seçimi temizle düğmesi aktif/pasif. Varsayılan false
        /// </summary>
        [HtmlAttributeName("brn-can-clear-select")]
        public bool CanClearSelect { get; set; }

        /// <summary>
        /// Bu bilgideki nesnenin value bilgisi Url bilgisinin sonunda eklenir.
        /// </summary>
        [HtmlAttributeName("brn-parent-combobox")]
        public string ParentComboBox { get; set; }

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

        /// <summary>
        /// select tagı için width belirler
        /// </summary>
        [HtmlAttributeName("brn-width")]
        public int Width { get; set; }



        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Select4TagHelper(IHtmlHelper htmlHelper)
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
            if (Width > 0)
                output.Attributes.Add("style", $"width:{Width}px");

            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            //if (!FieldName.IsEmpty())
            //{
            //    htmlId = FieldName;
            //    htmlName = FieldName;
            //}
            //if (!LabelText.IsEmpty())
            //    labelText = LabelText;

            var selectionJs = string.Empty;
            if (SelectedValues != null && SelectedValues.Count > 0)
            {
                var sb = new StringBuilder();
                foreach(var s in SelectedValues)
                {
                    sb.AppendLine($@"$('#{htmlId}').append(new Option('{s.Name}', {s.Id}, true, true));");
                }
                sb.AppendLine($@"$('#{htmlId}').trigger('change');");
                selectionJs = sb.ToString();
            }
            else
            {
                var valId = ModelItem != null && ModelItem.Model != null ? ModelItem.Model.ToString() : "";
                if (!valId.IsEmpty())
                {
                    selectionJs = $@"
 $.ajax('{LoaderUrl}/{valId}', {{dataType: 'json'}}).done(function(data) {{ 
    var option = new Option(data.text, data.id, true, true);
    $(""#{htmlId}"").append(option).trigger('change');
}});";
                }
            }

            string js = DisableJs
                ? ""
                : $@"<script type=""text/javascript"">
$(function () {{
    $(""#{htmlId}"").select2({{
        placeholder: ""{PlaceHolderText}"",
        language: ""tr"",
        {(CanClearSelect ? "allowClear: true, " : "")}
        {(MultiSelect ? "multiple:true," : "")}
        minimumInputLength: 1,
        ajax: {{
            url: '{Url}',
            data: function (params) {{
                return {{
                    q: params.term,
                    {(!ParentComboBox.IsEmpty() ? $"id: $('#{ParentComboBox}').val()," : "")}
                }};
            }},
            processResults: function (data) {{
                return {{ results: data }};
            }}
        }},
    }});
    {selectionJs}
}});
</script>";

            var valHtml = ModelItem != null ? "value='" + ModelItem.Model + "'" : "";
            if (DisableEditorTemplate)
            {
                if (!DisableColSize)
                    output.Attributes.Add("class", $"col-sm-{EditorColCount}");
                output.Attributes.Add("id", "div" + htmlId);
                if (!AddNewUrl.IsEmpty())
                {
                    var sep = "?";
                    if (AddNewUrl.Contains("?"))
                        sep = "&";
                    AddNewUrl += sep + "editorId=" + htmlId;
                    output.Content.SetHtmlContent($@"
<div class=""input-group input-group-sm"">
    <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
    </select>
    <div class=""input-group-append"">
        <a href='{AddNewUrl}' class='btn btn-sm btn-label-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
    </div>
    {js}
</div>");
                }
                else
                {
                    output.Content.SetHtmlContent($@"
<select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
</select>
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
        <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml}>
        </select>
        <div class=""input-group-append"">
            <a href='{AddNewUrl}' class='btn btn-sm btn-label-primary btnAddPopup fancyboxAdd fancybox.iframe'><i class='fas fa-plus'></i></a>
        </div>
    </div>
    {metaHtml}
</div>
{js}");
                }
                else
                {
                    output.Content.SetHtmlContent($@"
<label class=""col-sm-{LabelColCount} col-form-label col-form-label col-form-label-sm"">{labelText} {requiredHtml}</label>
<div class=""col-sm-{EditorColCount}"">
    <select id='{htmlId}' name='{htmlName}' class='form-control form-control-sm {CssClass}' {valHtml} >
    </select>
    {metaHtml}
</div>
{js}");
                }
            }
        }
    }
}
