using ReefDoctorId.Core.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class SpeciesTypeDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is SpeciesType type)
            {
                return type.ToFriendlyString();
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
