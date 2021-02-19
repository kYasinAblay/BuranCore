using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Buran.Core.MvcLibrary.XmlLang
{
    public class XmlLangStringLocalizer : IStringLocalizer
    {
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return XmlLangResource.GetAllResources().Select(x => new LocalizedString(x.Name, x.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        LocalizedString IStringLocalizer.this[string name] => XmlLangResource.GetResourceLocalize(name);

        LocalizedString IStringLocalizer.this[string name, params object[] arguments] => XmlLangResource.GetResourceLocalize(name, arguments);
    }
}
