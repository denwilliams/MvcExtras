using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.Web.Mvc
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns a string containing the Description attribute if available.
        /// If unavailable returns the ToString() value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute),
                                                       false);
            return attributes.Length == 0
                ? value.ToString()
                : ((DescriptionAttribute)attributes[0]).Description;
        }

        /// <summary>
        /// Gets a list of available values (ie: the enum values) for the specified enum.
        /// </summary>
        /// <param name="enumValue">The value to get available for.</param>
        /// <example>
        /// <code>
        /// Roles role = Role.Administrator;
        /// foreach (var roleType in role.GetAvailableValues())
        /// {
        ///   Console.WriteLine(roleType.ToString());
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public static Array GetAvailableValues(this Enum enumValue)
        {
            return enumValue.GetType().GetAvailableValues();
        }

        /// <summary>
        /// Gets the available values (ie: the enum values) for the specified enum type.
        /// </summary>
        /// <param name="enumType">The Type of enum.</param>
        /// <example>
        /// <code>
        /// foreach (var roleType in typeof(Roles).GetAvailableValues())
        /// {
        ///   Console.WriteLine(roleType.ToString());
        /// }
        /// </code>
        /// </example>
        /// <returns></returns>
        public static Array GetAvailableValues(this Type enumType)
        {
            return Enum.GetValues(enumType);
        }
    }
}
