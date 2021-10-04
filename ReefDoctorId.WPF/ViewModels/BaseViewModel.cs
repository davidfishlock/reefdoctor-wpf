using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IndicatorFlipcards.Services;
using Microsoft.Practices.ServiceLocation;
using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using ReefDoctorId.UWP;
using ReefDoctorId.WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ReefDoctorId.ViewModels
{
    public class BaseViewModel : ViewModelBase
    {
        private NavigationServiceEx _navigationService;

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

        public BaseViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();

            this.CanGoBack = false;
        }
        
        private RelayCommand _closeAppCommand;
        public RelayCommand CloseAppCommand => _closeAppCommand
                    ?? (_closeAppCommand = new RelayCommand(
                    () =>
                    {
                        Application.Current.Shutdown();
                    }));
    }
}