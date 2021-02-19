using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.XmlLang.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace Buran.Core.MvcLibrary.XmlLang
{
    public class XmlLangResource
    {
        private static string _defaultLang;
        private static Dictionary<string, XmlResource> _configStore;

        public static void LoadLanguages(IWebHostEnvironment hostingEnvironment, string langFolder, string defaultLang)
        {
            _defaultLang = defaultLang;
            _configStore = new Dictionary<string, XmlResource>();

            var folder = Path.Combine(hostingEnvironment.ContentRootPath, langFolder);
            var files = Directory.GetFiles(folder, "*.xml");
            foreach (var file in files)
            {
                var language = Path.GetFileNameWithoutExtension(file);
                if (language.IsEmpty())
                    continue;

                var serializer = new XmlSerializer(typeof(XmlResource));
                var tr = new StreamReader(file);
                var config = (XmlResource)serializer.Deserialize(tr);
                tr.Close();

                config.Items = config.Items.OrderBy(d => d.Name).ToList();

                _configStore.Add(language, config);
            }
        }

        public static string GetResource(string resourceName)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var langCode = cultureInfo.TwoLetterISOLanguageName;

            var ql = _defaultLang;
            if (_configStore.ContainsKey(langCode))
                ql = langCode;

            var val = _configStore[ql].Items.FirstOrDefault(d => d.Name == resourceName);
            return val == null ? resourceName : val.Value;
        }

        public static LocalizedString GetResourceLocalize(string resourceName)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var langCode = cultureInfo.TwoLetterISOLanguageName;

            var ql = _defaultLang;
            if (_configStore.ContainsKey(langCode))
                ql = langCode;

            var val = _configStore[ql].Items.FirstOrDefault(d => d.Name == resourceName);
            return new LocalizedString(resourceName,
                    val == null
                        ? resourceName
                        : val.Value);
        }

        public static LocalizedString GetResourceLocalize(string resourceName, params object[] arguments)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var langCode = cultureInfo.TwoLetterISOLanguageName;

            var ql = _defaultLang;
            if (_configStore.ContainsKey(langCode))
                ql = langCode;

            var val = _configStore[ql].Items.FirstOrDefault(d => d.Name == resourceName);
            return new LocalizedString(resourceName,
                    val == null
                        ? resourceName
                        : string.Format(val.Value, arguments));
        }

        public static List<Item> GetAllResources()
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var langCode = cultureInfo.TwoLetterISOLanguageName;

            return _configStore.ContainsKey(langCode)
                ? _configStore[langCode].Items
                : _configStore[_defaultLang].Items;
        }
    }
}
