using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class AsyncImageLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            var path = value as string;
            var bitmap = new BitmapImage() { CreateOptions = BitmapCreateOptions.IgnoreColorProfile };
            bitmap.BeginInit();

            var imageFile = new FileInfo(path);

            if (imageFile != null)
            {
                using (var infoStream = imageFile.OpenRead())
                {
                    if (infoStream != null && infoStream.Length > 0)
                    {
                        bitmap.BeginInit();
                        bitmap.StreamSource = infoStream;
                        bitmap.EndInit();
                    }
                }
            }

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
