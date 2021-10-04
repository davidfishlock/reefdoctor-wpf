using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReefDoctorId.UWP.Converters
{
    public sealed class AnswersItemNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            var nameString = String.Empty;

            var subject = value is Subject ? (Subject)value : null;

            if (subject  != null)
            {
                var isAnswersItem = (subject.Index == 0);

                var paramString = (string)parameter;

                switch (paramString)
                {
                    case "AnswersList":
                        nameString = isAnswersItem ? null : subject.Index + ". ";
                        break;
                    case "JumpList":
                        nameString = isAnswersItem ? "Answers" : subject.Index.ToString();
                        break;
                }
            }

            return nameString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
