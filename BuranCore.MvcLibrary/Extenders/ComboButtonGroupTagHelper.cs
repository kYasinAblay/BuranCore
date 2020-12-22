using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Data.Attributes;
using Buran.Core.MvcLibrary.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    [HtmlTargetElement("comboButtonGroup", Attributes = "brn-field")]
    public class ComboButtonGroupTagHelper : TagHelper
    {
        [HtmlAttributeName("brn-field")]
        public ModelExpression ModelItem { get; set; }

        [HtmlAttributeName("brn-items")]
        public SelectList ItemList { get; set; }

        [HtmlAttributeName("brn-items2")]
        public List<SelectListItem> ItemList2 { get; set; }



        private IHtmlHelper _htmlHelper;
        private IServiceProvider _serviceProvider;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public ComboButtonGroupTagHelper(IHtmlHelper htmlHelper, IServiceProvider provider)
        {
            _htmlHelper = htmlHelper;
            _serviceProvider = provider;
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

            var htmlId = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : _htmlHelper.IdForModel() + "_" + ModelItem.Metadata.PropertyName;
            var htmlName = prefix.IsEmpty() ? ModelItem.Metadata.PropertyName : prefix + "." + ModelItem.Metadata.PropertyName;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "input-group-btn");

            if (ItemList == null)
            {
                var comboDataInfo = GetComboBoxDataSource(ModelItem.ModelExplorer);
                ItemList = comboDataInfo.ListItems;
            }

            var selectedText = "";
            var selectedValue = 0;
            if (ModelItem.Model != null)
            {
                if (ModelItem.Model is int)
                {
                    selectedValue = int.Parse(ModelItem.Model.ToString());
                }
                else
                {
                    var ti = (int)ModelItem.Model;
                    selectedValue = ti;
                }
            }

            if (ItemList != null)
            {
                var it = ItemList.Where(d => d.Value == selectedValue.ToString()).FirstOrDefault();
                if (it == null)
                    it = ItemList.FirstOrDefault();
                if (it != null)
                {
                    selectedValue = int.Parse(it.Value);
                    selectedText = it.Text;
                }
            }
            else if (ItemList2 != null)
            {
                var it = ItemList2.Where(d => d.Value == selectedValue.ToString()).FirstOrDefault();
                if (it == null)
                    it = ItemList2.FirstOrDefault();
                if (it != null)
                {
                    selectedValue = int.Parse(it.Value);
                    selectedText = it.Text;
                }
            }

            var sbOptions = new StringBuilder();
            sbOptions.AppendLine($@"
<button type=""button"" class=""btn btn-outline-secondary btn-sm dropdown-toggle"" data-toggle=""dropdown"">
    <span id=""lbl{htmlId}"">{selectedText}</span> <span class=""caret""></span>
</button>
<div class=""dropdown-menu"">
");
            if (ItemList != null)
            {
                foreach (var item in ItemList)
                {
                    sbOptions.AppendLine($@"<a class=""dropdown-item"" href=""javascript:SelectComboGroup('{item.Text}', {item.Value}, '{htmlId}')"">{item.Text}</a>");
                }
            }
            else if (ItemList2 != null)
            {
                foreach (var item in ItemList2)
                {
                    sbOptions.AppendLine($@"<a class=""dropdown-item"" href=""javascript:SelectComboGroup('{item.Text}', {item.Value}, '{htmlId}')"">{item.Text}</a>");
                }
            }
            sbOptions.AppendLine($@"</div>
<input type=""hidden"" id=""{htmlId}"" name=""{htmlName}"" value=""{selectedValue}"" />");
            output.Content.SetHtmlContent(sbOptions.ToString());
        }
    }
}
