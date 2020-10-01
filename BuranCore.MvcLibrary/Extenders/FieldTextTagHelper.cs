using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("span", Attributes = "brn-fieldtext")]
    public class FieldTextTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-fieldtext")]
        public ModelExpression ModelItem { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var htmlFieldName = ModelItem.Metadata.PropertyName;
            var labelText = ModelItem.Metadata.DisplayName ?? ModelItem.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(labelText);
        }
    }
}
