using ReefDoctorId.Core.Models;
using ReefDoctorId.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ReefDoctorId.WPF.Views
{
    /// <summary>
    /// Interaction logic for SpeciesBrowserPage.xaml
    /// </summary>
    public partial class SpeciesBrowserPage : BasePage
    {
        private SpeciesBrowserViewModel _viewModel;

        public SpeciesBrowserPage()
        {
            this.InitializeComponent();
            this.PreviewKeyDown += SpeciesBrowserPage_PreviewKeyDown;

            this.Loaded += SpeciesBrowserPage_Loaded;
        }


        private void SpeciesBrowserPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel = this.DataContext as SpeciesBrowserViewModel;
        }

        private void SpeciesBrowserPage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (_viewModel.IsFullScreenVisible)
                    {
                        ImageOverlay.GoNext();
                    }

                    e.Handled = true;
                    break;
                case Key.Left:
                    if (_viewModel.IsFullScreenVisible)
                    {
                        ImageOverlay.GoBack();
                    }

                    e.Handled = true;
                    break;
            }
        }
    }
}
