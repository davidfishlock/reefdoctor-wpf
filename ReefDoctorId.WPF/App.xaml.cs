using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace ReefDoctorId.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
