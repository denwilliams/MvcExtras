using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MvcExtras.Utils;

// ### TABLE BUILDER USES WORK FROM 
// a) http://blogs.microsoft.co.il/blogs/gilf/archive/2009/01/13/extending-asp-net-mvc-htmlhelper-class.aspx
// b) http://stackoverflow.com/questions/6038255/asp-net-mvc-helpers-merging-two-object-htmlattributes-together
// and adds additional features of using IEnumerables, generics, display name & format string attributes, 
// and summary functions

namespace System.Web.Mvc
{
    public static partial class HtmlExtensions
    {
        /// <summary>
        /// Builds .
        /// </summary>
        /// <typeparam name="T">The type of items to be displayed in a table.</typeparam>
        /// <param name="helper">The HtmlHelper object.</param>
        /// <param name="items">The items to include in the table.</param>
        /// <param name="useDisplayName">if set to <c>true</c>, each property's DisplayName attribute will be used for the column header.
        /// if <c>false</c>, the Name attribute will be used. IF YOU DON'T SET the DisplayName attribute LEAVE THIS FALSE.</param>
        /// <param name="htmlAttributes">Optional HTML attributes to append to the table.</param>
        /// <param name="summaryFunctions">
        /// If specified, a footer row will be created using function specified here.
        /// Specify the class property name as a string for the dictionary key.
        /// </param>
        /// <param name="headerNames">
        /// If specified, these will be used to set header text. If not, the display name attribute, or property name is used.
        /// NOTE: it is possible to only specify the name for some columns. Attributes will be used for all other columns.
        /// </param>
        /// <returns></returns>
        public static MvcHtmlString ReportTable<T>(this HtmlHelper helper, IEnumerable<T> items, bool useDisplayName = false, object htmlAttributes = null, Dictionary<string, Func<IEnumerable<T>, string>> summaryFunctions = null, Dictionary<string, string> headerNames = null)
        {
            if (items == null || !items.Any())
            {
                return MvcHtmlString.Create(string.Empty);
            }

            return MvcHtmlString.Create(BuildTable(items, useDisplayName, htmlAttributes, summaryFunctions, headerNames));
        }



        private static string BuildTable<T>(IEnumerable<T> items, bool useDisplayName, object htmlAttributes, Dictionary<string, Func<IEnumerable<T>, string>> summaryFunctions = null, Dictionary<string, string> headerNames = null)
        {

            StringBuilder sb = new StringBuilder();

            var type = items.First().GetType();
            var properties = type.GetProperties();

            BuildTableHeader(sb, properties, useDisplayName, headerNames);
            bool oddrow = false;
            foreach (var item in items)
            {
                BuildTableRow(sb, item, (oddrow = !oddrow));
            }
            BuildSummaryRow<T>(sb, properties, items, summaryFunctions);

            TagBuilder builder = new TagBuilder("table");

            var dictAttributes = htmlAttributes.ToDictionary();
            builder.MergeAttributes(dictAttributes);

            //builder.MergeAttribute("name", name);

            builder.InnerHtml = sb.ToString();

            return builder.ToString(TagRenderMode.Normal);
        }



        private static void BuildTableRow(StringBuilder sb, object obj, bool oddrow)
        {
            Type objType = obj.GetType();
            sb.AppendFormat("\t<tr class={0}>",
                    oddrow ? "oddrow" : "evenrow");
            foreach (var property in objType.GetProperties())
            {
                object o = property.GetValue(obj, null);
                string s = null;
                string format = property.GetDisplayFormat();
                if (format != null)
                {
                    if (o is double)
                        s = ((double)o).ToString(format);
                    else if (o is int)
                        s = ((int)o).ToString(format);
                    else if (o is DateTime)
                        s = ((DateTime)o).ToString(format);
                    else if (o is DateTimeOffset)
                        s = ((DateTimeOffset)o).ToString(format);
                }
                if (s == null) s = (o == null ? "NULL" : o.ToString());
                sb.AppendFormat("\t\t<td>{0}</td>\n", s);
            }
            sb.AppendLine("\t</tr>");
        }



        private static void BuildTableHeader(StringBuilder sb, PropertyInfo[] properties, bool useDisplayName, Dictionary<string, string> headerNames = null)
        {
            sb.AppendLine("\t<tr class='header-row'>");
            foreach (var property in properties)
            {
                sb.AppendFormat("\t\t<th>{0}</th>\n", headerNames != null && headerNames.ContainsKey(property.Name) ? headerNames[property.Name] : useDisplayName ? property.GetDisplayName() : property.Name);
            }
            sb.AppendLine("\t</tr>");
        }

        private static void BuildSummaryRow<T>(StringBuilder sb, PropertyInfo[] properties, IEnumerable<T> tableData, Dictionary<string, Func<IEnumerable<T>, string>> summaryFunctions)
        {
            // don't make a summary row if we haven't got anything to calculate
            if (summaryFunctions == null)
                return;

            // don't make a summary row unless we have at least 2 items
            if (tableData.Count() < 2)
                return;

            sb.AppendLine("\t<tr class='summary-row'>");

            // For each property in the type, check if a summary function is defined.
            // If it is defined, then use it. If not, then leave a blank cell.
            foreach (var property in properties)
            {
                if (summaryFunctions.ContainsKey(property.Name))
                {
                    try
                    {
                        var func = summaryFunctions[property.Name];
                        sb.AppendFormat("\t\t<td>{0}</td>\n", func(tableData));
                    }
                    catch
                    {
                        sb.Append("\t\t<td>ERROR</td>\n");
                    }
                }
                else
                {
                    sb.Append("\t\t<td></td>\n");
                }
            }

            sb.AppendLine("\t</tr>");
        }

        public static IDictionary<string, object> ToDictionary(this object data)
        {
            if (data == null) return null; // Or throw an ArgumentNullException if you want

            BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo property in data.GetType().GetProperties(publicAttributes))
            {
                if (property.CanRead)
                {
                    dictionary.Add(property.Name, property.GetValue(data, null));
                }
            }
            return dictionary;
        }
    }
}
