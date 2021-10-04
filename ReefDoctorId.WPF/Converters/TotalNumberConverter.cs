using System;
using System.Globalization;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class TotalNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value != null)
            {
                var totalItems = (int)value;

                return totalItems - 1;
            }

            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
