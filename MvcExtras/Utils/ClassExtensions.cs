using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvcExtras.Utils
{
    internal static class ClassExtensions
    {
        public static string GetDisplayName(this object obj, string propertyName)
        {
            if (obj == null) return null;
            return GetDisplayName(obj.GetType(), propertyName);

        }

        //public static string GetDisplayName(this Type type, string propertyName)
        //{
        //    var property = type.GetProperty(propertyName);
        //    if (property == null) return null;

        //    return GetDisplayName(property);
        //}

        public static string GetDisplayName(this Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            if (property == null) return null;

            return GetDisplayName(property);
        }

        public static string GetDisplayName(this PropertyInfo property)
        {
            var attrName = GetAttributeDisplayName(property);
            if (!string.IsNullOrEmpty(attrName))
                return attrName;

            var metaName = GetMetaDisplayName(property);
            if (!string.IsNullOrEmpty(metaName))
                return metaName;

            return property.Name.ToString(CultureInfo.InvariantCulture);
        }


        private static string GetAttributeDisplayName(this PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayNameAttribute), true);
            if (atts.Length == 0)
                return null;
            var displayNameAttribute = atts[0] as DisplayNameAttribute;
            return displayNameAttribute != null ? displayNameAttribute.DisplayName : null;
        }

        public static string GetDisplayFormat(this PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(
                typeof(DisplayFormatAttribute), true);
            if (atts.Length == 0)
                return null;
            var displayFormatAttribute = atts[0] as DisplayFormatAttribute;
            return displayFormatAttribute != null ? displayFormatAttribute.DataFormatString : null;
        }

        private static string GetMetaDisplayName(this PropertyInfo property)
        {
            if (property.DeclaringType != null)
            {
                var atts = property.DeclaringType.GetCustomAttributes(
                    typeof(MetadataTypeAttribute), true);
                if (atts.Length == 0)
                    return null;

                var metaAttr = atts[0] as MetadataTypeAttribute;
                if (metaAttr != null)
                {
                    var metaProperty =
                        metaAttr.MetadataClassType.GetProperty(property.Name);
                    return metaProperty == null ? null : GetAttributeDisplayName(metaProperty);
                }
            }
            return null;
        }
    }
}
