using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class FlexWrapPanelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (!(value is double @width) || @width == 0)
            {
                return 0;
            }

            if (@width > 1150)
            {
                return (@width - 24) / 3;
            }
            else if (@width > 780)
            {
                return (@width - 12) / 2;
            }
            else
            {
                return @width - 12;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
