using Buran.Core.Library.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("div", Attributes = "brn-field")]
    public class DivGroupTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        [HtmlAttributeName("brn-prefix")]
        public string Prefix { get; set; }

        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public DivGroupTagHelper(IHtmlHelper htmlHelper)
        {
            Prefix = "div";  
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            var prefix2 = ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix;
            var htmlId = prefix2.IsEmpty()
                ? ModelItem.Metadata.PropertyName
                : _htmlHelper.IdForModel() != ModelItem.Metadata.PropertyName
                    ? _htmlHelper.IdForModel() + "_" + ModelItem.Metadata.PropertyName
                    : ModelItem.Metadata.PropertyName;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("id", Prefix + htmlId);
        }
    }
}
