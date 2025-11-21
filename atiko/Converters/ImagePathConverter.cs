using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.Storage;

namespace atiko.Converters;

public partial class ImagePathConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string path && !string.IsNullOrEmpty(path))
        {
            try
            {
                var bitmapImage = new BitmapImage();
                var file = StorageFile.GetFileFromPathAsync(path).AsTask().Result;
                using (var stream = file.OpenReadAsync().AsTask().Result)
                {
                    bitmapImage.SetSource(stream);
                }
                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}
