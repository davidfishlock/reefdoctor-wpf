using ReefDoctorId.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class MainBGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            var rnd = new Random();

            return new Uri("/Assets/Backgrounds/bg" + rnd.Next(1, 4) + ".jpg", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
