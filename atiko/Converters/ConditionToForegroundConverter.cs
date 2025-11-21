using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace atiko.Converters;

public partial class ConditionToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        if (value is string condition)
        {
            return condition switch
            {
                "Mint" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 22, 101, 52)),
                "Excellent" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 146, 64, 14)),
                "Good" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 29, 78, 216)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 75, 85, 99))
            };
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 75, 85, 99));
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
