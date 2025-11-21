using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;

public partial class DateTimeToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            string format = parameter?.ToString() ?? "dd/MM/yyyy";
            return dateTimeOffset.ToString(format);
        }
        else if (value is DateTime dateTime)
        {
            string format = parameter?.ToString() ?? "dd/MM/yyyy";
            return dateTime.ToString(format);
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
        {
            string format = parameter?.ToString() ?? "dd/MM/yyyy";
            try
            {
                DateTime parsedDateTime = DateTime.ParseExact(stringValue, format, System.Globalization.CultureInfo.InvariantCulture);
                return new DateTimeOffset(parsedDateTime, TimeSpan.Zero);
            }
            catch (FormatException)
            {
                return null;
            }
        }
        return null;
    }
}