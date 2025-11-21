using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace atiko.Converters;

public partial class ModeToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isActive && isActive)
        {
            return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 185, 156, 138)); // #B99C8A
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 198, 177, 161)); // #C6B1A1 (неактивний)
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}