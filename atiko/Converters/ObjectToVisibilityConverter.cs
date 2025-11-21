using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            if (value is string str)
                return string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;

            if (value is bool boolValue)
                return boolValue ? Visibility.Visible : Visibility.Collapsed;

            if (value is int)
                return Visibility.Visible;

            if (value is DateTime)
                return Visibility.Visible;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
