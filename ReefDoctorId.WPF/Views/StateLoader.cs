using System;
using System.Windows;
using System.Windows.Controls;

namespace ReefDoctorId.WPF.Views
{
    public class StateLoader : DependencyObject
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State",
                                        typeof(String),
                                        typeof(StateLoader),
                                        new PropertyMetadata(String.Empty,
                                            new PropertyChangedCallback(OnStateChanged)));

        public static void SetState(DependencyObject obj, String value)
        {
            obj.SetValue(StateProperty, value);
        }

        public static String GetState(Control obj)
        {
            return (String)obj.GetValue(StateProperty);
        } 

        internal static void OnStateChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
                VisualStateManager.GoToState((FrameworkElement)target, args.NewValue.ToString(), true);
        }
    }
}
