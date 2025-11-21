using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is bool booleanValue ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is Visibility visibility ? visibility == Visibility.Visible : false;
}
