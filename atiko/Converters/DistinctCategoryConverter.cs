using atiko.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace atiko.Converters;
public partial class DistinctCategoryConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, string language)
    {
        if (value is IEnumerable<VintageItem> items)
        {
            return new List<string?> { null }
                .Concat(items.Select(i => i.Category).Where(c => c != null).Distinct().OrderBy(c => c))
                .ToList();
        }
        return new List<string?> { null };
    }

    public object ConvertBack(object value, Type targetType, object? parameter, string language) =>
        throw new NotSupportedException();
}
