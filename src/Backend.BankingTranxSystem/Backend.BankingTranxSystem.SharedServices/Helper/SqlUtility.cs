using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Backend.BankingTranxSystem.SharedServices.Helper;

public static class SqlUtilityExtensions
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> items)
    {
        var tb = new DataTable(typeof(T).Name);

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach ( var prop in props )
        {
            var exclude = Attribute.GetCustomAttribute(prop, typeof(ExcludePropertyAttribute)) as ExcludePropertyAttribute;

            if( exclude is null)
                tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in items)
        {
            var values = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null) ?? DBNull.Value;
            }

            tb.Rows.Add(values);
        }

        return tb;
    }

    public class ExcludePropertyAttribute : Attribute
    {

    }
}