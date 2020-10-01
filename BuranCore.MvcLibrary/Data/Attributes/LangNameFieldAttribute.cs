using System;

namespace Buran.Core.MvcLibrary.Data.Attributes
{
    public class LangNameFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public LangNameFieldAttribute()
        {
            Name = "Name";
        }
    }
}
