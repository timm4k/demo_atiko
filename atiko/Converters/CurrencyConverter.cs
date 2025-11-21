using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return $"${doubleValue:F2}";
        }
        if (value is decimal decimalValue)
        {
            return $"${decimalValue:F2}";
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("ConvertBack is not implemented for CurrencyConverter");
}
