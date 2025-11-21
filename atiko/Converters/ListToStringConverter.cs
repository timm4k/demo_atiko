using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;

namespace atiko.Converters;

public class ListToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is List<string> list)
        {
            return string.Join(", ", list);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}