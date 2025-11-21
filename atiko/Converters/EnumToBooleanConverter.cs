using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;
public partial class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        if (value == null || parameter is not string enumString) return false;
        try
        {
            var enumValue = Enum.Parse(value.GetType(), enumString);
            return value.Equals(enumValue);
        }
        catch
        {
            return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language)
    {
        if (value is not bool boolValue || !boolValue || parameter is not string enumString)
            return Enum.GetValues(targetType).GetValue(0)!;
        try
        {
            return Enum.Parse(targetType, enumString);
        }
        catch
        {
            return Enum.GetValues(targetType).GetValue(0)!;
        }
    }
}