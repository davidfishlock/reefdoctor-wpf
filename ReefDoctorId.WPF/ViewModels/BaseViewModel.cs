using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IndicatorFlipcards.Services;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace ReefDoctorId.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private bool _canGoBack;
        public bool CanGoBack
        {
            get
            {
                return _canGoBack;
            }
            set
            {
                if (value != _canGoBack)
                {
                    _canGoBack = value;
                    RaisePropertyChanged("CanGoBack");
                }
            }
        }

        protected NavigationServiceEx NavigationService;

        public BaseViewModel()
        {
            NavigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();

            CanGoBack = false;
        }

        private RelayCommand _closeAppCommand;
        public RelayCommand CloseAppCommand => _closeAppCommand
                    ?? (_closeAppCommand = new RelayCommand(
                    () =>
                    {
                        Application.Current.Shutdown();
                    }));

        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand => _goBackCommand
                    ?? (_goBackCommand = new RelayCommand(
                    () =>
                    {
                        if (!CanGoBack)
                        {
                            throw new System.Exception("Attempted to navigate back while CanGoBack is false.");
                        }

                        NavigationService.GoBack();
                    }));
    }
}