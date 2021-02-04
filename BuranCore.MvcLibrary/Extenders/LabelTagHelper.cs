using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Reflection;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class RequiredUtils
    {
        public static bool GetIsRequired(this ModelMetadata model)
        {
            var rq = Digger2.GetMetaAttr<RequiredAttribute>(model);
            return rq != null;
        }
    }
    [HtmlTargetElement("label", Attributes = "brn-field")]
    public class LabelTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        [HtmlAttributeName("brn-suppress")]
        public bool SupressRequired { get; set; }

        [HtmlAttributeName("brn-required-css-class")]
        public string RequiredCssClass { get; set; }

        [HtmlAttributeName("brn-symbol")]
        public string Symbol { get; set; }

        [HtmlAttributeName("brn-notext")]
        public bool NoText { get; set; }

        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public LabelTagHelper(IHtmlHelper htmlHelper)
        {
            RequiredCssClass = "editor-field-required";
            Symbol = " *";
            _htmlHelper = htmlHelper;
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

            if (htmlId.EndsWith("_"))
                htmlId = htmlId.Substring(0, htmlId.Length - 1);

            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlId.Split('.').Last();

            output.TagName = "label";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("for", htmlId);

            if (!NoText)
            {
                if (!SupressRequired && ModelItem.Metadata.GetIsRequired())
                {
                    var required = new TagBuilder("span");
                    required.AddCssClass(RequiredCssClass);
                    required.InnerHtml.AppendHtml(Symbol);

                    output.Content.SetHtmlContent(labelText + required.GetString());
                }
                else
                    output.Content.SetHtmlContent(labelText);
            }
        }
    }
}
