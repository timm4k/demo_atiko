using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
            {
                return intValue.ToString();
            }
            else if (value == null)
            {
                return "";
            }
            return "";
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string str && int.TryParse(str, out int result))
            {
                return result;
            }
            return null;
        }
    }
}