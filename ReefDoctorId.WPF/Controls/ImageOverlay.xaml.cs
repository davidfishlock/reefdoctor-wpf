using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ReefDoctorId.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ImageOverlay.xaml
    /// </summary>
    public partial class ImageOverlay : OverlayBase
    {
        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(List<string>), typeof(ImageOverlay), new PropertyMetadata(new List<string>()));

        public ImageOverlay()
        {
            InitializeComponent();
        }

        public List<string> ItemsSource
        {
            get
            {
                return (List<string>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public void GoNext()
        {
            if (ImageFlipper.IsFlipEnabled && ImageFlipper.SelectedIndex < ImageFlipper.Items.Count - 1)
            {
                ImageFlipper.SelectedIndex += 1;
            }
        }

        public void GoBack()
        {
            if (ImageFlipper.IsFlipEnabled && ImageFlipper.SelectedIndex > 0)
            {
                ImageFlipper.SelectedIndex -= 1;
            }
        }
    }
}
