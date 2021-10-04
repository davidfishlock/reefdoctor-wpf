using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class HasInfoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            var subject = (Subject)value;
            var returnValue = (subject != null && ((subject.Info != null && subject.Info.Count > 0) || (subject.Similar != null && subject.Similar.Count > 0)));

            if (parameter != null)
            {
                returnValue = !returnValue;
            }

            return returnValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
