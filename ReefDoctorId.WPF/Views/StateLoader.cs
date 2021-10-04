using System.Windows;
using System.Windows.Controls;

namespace ReefDoctorId.WPF.Views
{
    public class StateLoader : DependencyObject
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State",
                                        typeof(string),
                                        typeof(StateLoader),
                                        new PropertyMetadata(string.Empty,
                                            new PropertyChangedCallback(OnStateChanged)));

        public static void SetState(DependencyObject obj, string value)
        {
            obj.SetValue(StateProperty, value);
        }

        public static string GetState(Control obj)
        {
            return (string)obj.GetValue(StateProperty);
        }

        internal static void OnStateChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
                VisualStateManager.GoToState((FrameworkElement)target, args.NewValue.ToString(), true);
        }
    }
}
