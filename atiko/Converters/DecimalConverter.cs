using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters
{
    public partial class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal dec)
                return dec.ToString("F2");
            if (value is double d)
                return d.ToString("F2");
            return "0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string str && decimal.TryParse(str, out decimal result))
            {
                return result;
            }
            return 0m;
        }
    }
}