using GalaSoft.MvvmLight.Ioc;
using IndicatorFlipcards.Services;
using Microsoft.Practices.ServiceLocation;
using ReefDoctorId.WPF.Models;
using ReefDoctorId.WPF.Views;

namespace ReefDoctorId.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => new NavigationServiceEx());
            SimpleIoc.Default.Register(() => new SpeciesDataModel());
            Register<MainViewModel, MainPage>();
            Register<WorkshopViewModel, WorkshopPage>();
            Register<SpeciesBrowserViewModel, SpeciesBrowserPage>();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();
        public WorkshopViewModel WorkshopViewModel => ServiceLocator.Current.GetInstance<WorkshopViewModel>();
        public SpeciesBrowserViewModel SpeciesBrowserViewModel => ServiceLocator.Current.GetInstance<SpeciesBrowserViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();
        }
    }
}
