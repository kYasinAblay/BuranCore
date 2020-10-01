using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Buran.Core.Library.Reflection
{
    public static class Digger
    {
        public static Type GetType(Type type, string propertyName)
        {
            if (type == null && propertyName.Contains("."))
            {
                return null;
            }

            var fields = propertyName.Split('.');
            for (int i = 0; i < fields.Count() - 1; i++)
            {
                var lazyProperty = type.GetProperty(fields[i]);
                if (lazyProperty != null)
                {
                    type = lazyProperty.PropertyType;
                }
            }
            propertyName = fields[fields.Count() - 1];
            if (type != null)
            {
                var prop = type.GetProperty(propertyName);
                return prop != null ? type.GetProperty(propertyName).PropertyType : null;
            }
            return null;
        }

        public static dynamic GetObjectValue(dynamic source, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var fields = propertyName.Split('.');
            for (int i = 0; i < fields.Count() - 1; i++)
            {
                if (source == null)
                {
                    continue;
                }

                var lazyProperty = source.GetType().GetProperty(fields[i]);
                if (lazyProperty != null)
                {
                    source = lazyProperty.GetValue(source, null);
                }
            }
            propertyName = fields[fields.Count() - 1];
            if (source != null)
            {
                if (source is ExpandoObject)
                {
                    var dict = source as IDictionary<string, object>;
                    return dict.ContainsKey(propertyName) ? dict[propertyName] : string.Empty;
                }
                var property = source.GetType().GetProperty(propertyName);
                return property != null ? property.GetValue(source, null) : null;
            }
            return null;
        }

        public static void SetObjectValue(object source, string propertyName, object value)
        {
            var property = source.GetType().GetProperty(propertyName);
            if (property != null)
            {
                property.SetValue(source, value, null);
            }
        }

        public static string GetDisplayName(Type type, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return string.Empty;
            }

            var fields = fieldName.Split('.');
            if (fields.Count() > 1)
            {
                for (int i = 0; i < fields.Count() - 1; i++)
                {
                    var lazyProperty = type.GetProperty(fields[i]);
                    if (lazyProperty != null)
                    {
                        type = lazyProperty.PropertyType;
                    }
                }
                fieldName = fields[fields.Count() - 1];
            }
            return fieldName;
        }

        public static string GetClassName(Type type, bool clearTire = false)
        {
            var r = type.Name;
            if (r.IndexOf("_", StringComparison.Ordinal) > -1)
            {
                r = r.Split('_')[0];
            }

            return r;
        }

        public static bool IsTypeOf(Type source, Type target)
        {
            while (true)
            {
                if (source == target)
                {
                    return true;
                }

                if (source.BaseType != null)
                {
                    source = source.BaseType;
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        public static List<string> GetAttributePropertyList<T>(Type type) where T : class
        {
            var list = new List<string>();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var exists = prop.GetCustomAttributes(typeof(T), false).Any();
                if (exists)
                {
                    list.Add(prop.Name);
                }
            }
            return list;
        }

        public static Dictionary<string, T> GetAttributePropertyList2<T>(Type type) where T : class
        {
            var list = new Dictionary<string, T>();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var data = prop.GetCustomAttributes(typeof(T), false).ToList();
                if (data.Any())
                {
                    list.Add(prop.Name, data.First() as T);
                }
            }
            return list;
        }

        public static T GetAttribute<T>(Type type, bool searchProperties = true) where T : class
        {
            if (searchProperties)
            {
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var propList = prop.GetCustomAttributes(typeof(T), false);
                    if (propList.Any())
                    {
                        return propList.First() as T;
                    }
                }
            }
            else
            {
                var propList = type.GetCustomAttributes(typeof(T), false);
                if (propList.Any())
                {
                    return propList.First() as T;
                }
            }
            return null;
        }

        public static T[] GetAttributeList<T>(Type type, bool searchProperties = true) where T : class
        {
            if (searchProperties)
            {
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var propList = prop.GetCustomAttributes(typeof(T), false);
                    if (propList.Count() > 0)
                    {
                        return propList as T[];
                    }
                }
            }
            else
            {
                var propList = type.GetCustomAttributes(typeof(T), false);
                if (propList.Count() > 0)
                {
                    return propList as T[];
                }
            }
            return null;
        }
    }
}
