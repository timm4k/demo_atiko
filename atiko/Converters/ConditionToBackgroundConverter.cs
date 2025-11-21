using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace atiko.Converters;

public partial class ConditionToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        if (value is string condition)
        {
            return condition switch
            {
                "Mint" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 220, 252, 231)),
                "Excellent" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 254, 243, 199)),
                "Good" => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 219, 234, 254)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 244, 246))
            };
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 244, 246));
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
