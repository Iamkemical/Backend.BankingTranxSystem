using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System;

namespace Backend.BankingTranxSystem.SharedServices.Extensions;
public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        object[] attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
        if (attribs.Length > 0)
        {
            return ((DescriptionAttribute)attribs[0]).Description;
        }
        return value.ToString();
    }

    public static string GetDescription<T>(this T enumValue) where T : IComparable, IFormattable, IConvertible
    {

        if (!Enum.IsDefined(typeof(T), enumValue))
            return string.Empty;

        var displayAttribute = enumValue.GetType()
                                                     .GetMember(enumValue.ToString())
                                                     .First()
                                                     .GetCustomAttribute<DescriptionAttribute>();

        string displayName = displayAttribute?.Description;

        return displayName ?? enumValue.ToString();
    }
    public static T[] GetEnumTypes<T>(this Type enumType)
    {
        return (from name in Enum.GetNames(enumType)
                let enumValue = Enum.Parse(enumType, name)
                select (T)enumValue).ToArray();
    }
}