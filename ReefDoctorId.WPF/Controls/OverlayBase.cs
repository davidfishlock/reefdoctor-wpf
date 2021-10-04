using ReefDoctorId.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReefDoctorId.WPF.Controls
{
    public class OverlayBase : UserControl
    {
        private Page _parent;

        public static DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(OverlayBase), new PropertyMetadata(false));
        public static DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(OverlayBase), new PropertyMetadata(null));

        public OverlayBase()
        {
            this.Loaded += OverlayBase_Loaded;
            this.Unloaded += OverlayBase_Unloaded;
        }

        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return (ICommand)GetValue(CloseCommandProperty);
            }
            set
            {
                SetValue(CloseCommandProperty, value);
            }
        }
        
        private void OverlayBase_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is Page))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            _parent = (Page)parent;
            _parent.PreviewKeyDown += OverlayBase_PreviewKeyDown;
        }

        private void OverlayBase_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.PreviewKeyDown -= OverlayBase_PreviewKeyDown;
        }

        private void OverlayBase_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var workshopVM = this.DataContext as WorkshopViewModel;

            if (this.GetType() != typeof(InfoDialog) || (workshopVM != null && workshopVM.IsFullScreenVisible == false))
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        if (this.IsOpen && this.CloseCommand != null)
                        {
                            this.CloseCommand.Execute(null);
                            e.Handled = true;
                        }
                        break;
                }
            }
        }
    }
}
