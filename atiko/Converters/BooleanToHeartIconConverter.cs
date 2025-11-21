using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class BooleanToHeartIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        return value is bool isFavorite && isFavorite
            ? new FontIcon { Glyph = "\uEB52" }
            : new FontIcon { Glyph = "\uEB51" };
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
