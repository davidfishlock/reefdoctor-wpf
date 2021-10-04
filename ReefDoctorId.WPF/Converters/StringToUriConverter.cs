using ReefDoctorId.Core.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value != null)
            {
                var inputString = (string)value;
                return new Uri(inputString, UriKind.Absolute);
            }

            return new Uri("http://www.whatever.com/no.jpg", UriKind.Absolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
