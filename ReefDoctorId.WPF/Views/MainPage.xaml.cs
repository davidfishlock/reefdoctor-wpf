using System;
using System.Windows;
using System.Windows.Navigation;

namespace ReefDoctorId.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : BasePage
    {
        private static bool introSeen = false;

        public MainPage()
        {
            InitializeComponent();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var navWindow = Window.GetWindow(this) as NavigationWindow;
                if (navWindow != null) navWindow.ShowsNavigationUI = false;
            }));

            Intro.Opacity = 1;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (introSeen == false)
            {
                VisualStateManager.GoToElementState(RootGrid, "Splash", false);
            }
        }

        private void Splash_Completed(object sender, EventArgs e)
        {
            VisualStateManager.GoToElementState(RootGrid, "Menu", false);
            introSeen = true;
        }

        private void MenuShow_Completed(object sender, EventArgs e)
        {
            MainMenu.ShowIntro();
        }
    }
}
