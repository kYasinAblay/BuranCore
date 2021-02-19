using System.Collections.Generic;
using System.Xml.Serialization;

namespace Buran.Core.MvcLibrary.XmlLang.Models
{
    public class XmlResource
    {
        public List<Item> Items { get; set; }

        public XmlResource()
        {
            Items = new List<Item>();
        }
    }

    public class Item
    {
        [XmlAttribute]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}