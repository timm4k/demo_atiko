using Microsoft.UI.Xaml.Data;
using System;

namespace atiko.Converters;
public partial class BooleanToSortDirectionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        bool isAscending = value is bool asc && asc;
        string format = parameter as string ?? "full";
        return format == "short" ? (isAscending ? "Ascending" : "Descending") : (isAscending ? "Sort Ascending" : "Sort Descending");
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
