using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace atiko.Converters;

public partial class BooleanToHeartColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        return new SolidColorBrush(
            value is bool isFavorite && isFavorite
            ? Windows.UI.Color.FromArgb(255, 239, 68, 68)
            : Windows.UI.Color.FromArgb(255, 107, 114, 128)
        );
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
