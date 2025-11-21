using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters
{
    public partial class ConditionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return false as object;

            string? condition = value.ToString();
            string? expectedCondition = parameter.ToString();

            if (condition == null || expectedCondition == null)
                return false as object;

            return string.Equals(condition, expectedCondition, StringComparison.OrdinalIgnoreCase) as object;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked && parameter != null)
            {
                return parameter.ToString() ?? string.Empty;
            }
            return null;
        }
    }
}