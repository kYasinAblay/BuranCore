using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Buran.Core.Library.Utils
{
    public static class DynamicExtensions
    {
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
            {
                expando.Add(property.Name, property.GetValue(value));
            }

            return expando as ExpandoObject;
        }

        public static void AddProp(ExpandoObject o, string prop, object value)
        {
            var x = o as IDictionary<string, object>;
            x.Add(prop, value);
        }
    }

}
