using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class FlexWrapPanelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value != null && value is double)
            {
                var width = (double)value;

                Debug.WriteLine(value);

                if (width > 1150)
                {
                    return (width - 24) / 3;
                }
                else if (width > 780)
                {
                    return (width - 12) / 2;
                }
                else
                {
                    return width - 12;
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
