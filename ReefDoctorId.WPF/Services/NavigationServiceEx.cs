using ReefDoctorId.WPF.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IndicatorFlipcards.Services
{
    public class NavigationServiceEx
    {
        private NavigationWindow _mainWindow;
        public NavigationWindow MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = Application.Current.MainWindow as NavigationWindow;
                }

                return _mainWindow;
            }
            set
            {
                _mainWindow = value;
            }
        }

        public bool CanGoBack => MainWindow.CanGoBack;
        public bool CanGoForward => MainWindow.CanGoForward;

        public void GoBack() => MainWindow.GoBack();
        public void GoForward() => MainWindow.GoForward();

        public bool Navigate(string path, object parameter = null)
        {
            var navigationResult = MainWindow.Navigate(new Uri(path, UriKind.Relative), parameter);

            MainWindow.Navigated += Frame_Navigated;
            return navigationResult;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            MainWindow.Navigated -= Frame_Navigated;

            if (MainWindow.Content is BasePage page)
            {
                page.NavigationData = e.ExtraData;
            }
        }
    }
}
