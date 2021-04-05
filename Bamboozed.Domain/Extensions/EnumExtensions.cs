using System;
using System.ComponentModel;
using System.Reflection;

namespace Bamboozed.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnumByDescription<T>(this string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }

        public static string GetDescription<T>(this T value) where T : Enum
        {
            var enumType = typeof(T);
            var enumValue = Enum.GetName(enumType, value);

            var memberInfo = enumType.GetMember(enumValue);

            if ((memberInfo.Length <= 0)) return enumValue;

            var descriptionAttribute = (DescriptionAttribute)memberInfo[0].GetCustomAttribute(typeof(DescriptionAttribute), false);
            return descriptionAttribute != null ? descriptionAttribute.Description : enumValue;
        }
    }
}
