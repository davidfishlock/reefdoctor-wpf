using ReefDoctorId.Core.Models;
using ReefDoctorId.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ReefDoctorId.WPF.Views
{
    /// <summary>
    /// Interaction logic for WorkshopPage.xaml
    /// </summary>
    public partial class WorkshopPage : BasePage
    {
        private WorkshopViewModel _viewModel;

        public WorkshopPage()
        {
            this.InitializeComponent();
        }

        private void WorkshopPage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Right:
                    if (_viewModel.IsFullScreenVisible)
                    {
                        ImageOverlay.GoNext();
                    }
                    else if (!_viewModel.IsInfoVisible)
                    {
                        if (ItemsFlipper.SelectedIndex < ItemsFlipper.Items.Count - 1)
                        {
                            ItemsFlipper.SelectedIndex += 1;
                        }
                    }

                    e.Handled = true;
                    break;
                case Key.Left:
                    if (_viewModel.IsFullScreenVisible)
                    {
                        ImageOverlay.GoBack();
                    }
                    else if (!_viewModel.IsInfoVisible)
                    {
                        if (ItemsFlipper.SelectedIndex > 0)
                        {
                            ItemsFlipper.SelectedIndex -= 1;
                        }
                    }
                    
                    e.Handled = true;
                    break;
                case Key.Space:
                    if (_viewModel.LaunchContext.ExerciseType == ExerciseType.Workshop)
                    {
                        _viewModel.SelectedItem.IsNameVisible = !_viewModel.SelectedItem.IsNameVisible;
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void ItemsFlipper_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.HideNameCommand.Execute(null);
            }
        }

        private void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel = this.DataContext as WorkshopViewModel;

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (_viewModel != null)
                {
                    _viewModel.LaunchContext = (LaunchContext)this.NavigationData;
                }

                this.PreviewKeyDown += WorkshopPage_KeyDown;
                this.Focus();

                ItemsFlipper.SelectedIndex = 0;
            }));
        }

        private void BasePage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.PreviewKeyDown -= WorkshopPage_KeyDown;
            _viewModel.TearDown();
        }
    }
}
