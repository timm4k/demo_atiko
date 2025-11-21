using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class BooleanToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            string[] weights = parameter?.ToString()?.Split('|') ?? new[] { "Normal", "Bold" };
            string falseWeight = weights.Length > 0 ? weights[0] : "Normal";
            string trueWeight = weights.Length > 1 ? weights[1] : "Bold";
            return boolValue ? FontWeights.Bold : FontWeights.Normal;
        }
        return FontWeights.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException("ConvertBack is not implemented for BooleanToFontWeightConverter");
}
