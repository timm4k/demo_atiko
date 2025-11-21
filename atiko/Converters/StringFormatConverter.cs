using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class StringFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string formatString && value != null)
        {
            if (value is DateTime dateTime)
            {
                return string.Format(formatString, dateTime.ToString("yyyy-MM-dd"));
            }
            return string.Format(formatString, value);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}