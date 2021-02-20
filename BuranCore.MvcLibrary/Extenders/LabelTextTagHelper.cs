using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("label", Attributes = "brn-text")]
    public class LabelTextTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-text")]
        public string Text { get; set; }

        [HtmlAttributeName("brn-name")]
        public string Name { get; set; }

        [HtmlAttributeName("brn-isrequired")]
        public bool IsRequired { get; set; }

        [HtmlAttributeName("brn-required-css-class")]
        public string RequiredCssClass { get; set; }

        [HtmlAttributeName("brn-symbol")]
        public string Symbol { get; set; }


        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public LabelTextTagHelper(IHtmlHelper htmlHelper)
        {
            RequiredCssClass = "editor-field-required";
            Symbol = " *";
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);

            output.TagName = "label";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("for", Name);

            if (IsRequired)
            {
                var required = new TagBuilder("span");
                required.AddCssClass(RequiredCssClass);
                required.InnerHtml.AppendHtml(Symbol);

                output.Content.SetHtmlContent(Text + required.GetString());
            }
            else
                output.Content.SetHtmlContent(Text);
        }
    }
}
