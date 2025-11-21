using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;
public partial class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language) =>
        string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
