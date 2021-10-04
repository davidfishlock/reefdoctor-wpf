using GalaSoft.MvvmLight.Command;
using ReefDoctorId.UWP;
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

namespace ReefDoctorId.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private NavigationServiceEx _navigationService;
        private SpeciesDataModel _speciesDataModel;
        private LaunchContext _launchContext;
        private string _correctPassword = "nudibranch";

        public MainViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            _speciesDataModel = ServiceLocator.Current.GetInstance<SpeciesDataModel>();

            this.CanGoBack = false;

            this.Initialize();
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading == value)
                {
                    return;
                }

                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        private bool _isLevelDialogOpen;
        public bool IsLevelDialogOpen
        {
            get
            {
                return _isLevelDialogOpen;
            }
            set
            {
                if (value != _isLevelDialogOpen)
                {
                    _isLevelDialogOpen = value;
                    RaisePropertyChanged("IsLevelDialogOpen");
                }
            }
        }

        private bool _isExamDialogOpen;
        public bool IsExamDialogOpen
        {
            get
            {
                return _isExamDialogOpen;
            }
            set
            {
                if (value != _isExamDialogOpen)
                {
                    _isExamDialogOpen = value;
                    RaisePropertyChanged("IsExamDialogOpen");
                }
            }
        }

        private bool _isPasswordDialogOpen;
        public bool IsPasswordDialogOpen
        {
            get
            {
                return _isPasswordDialogOpen;
            }
            set
            {
                if (value != _isPasswordDialogOpen)
                {
                    _isPasswordDialogOpen = value;
                    RaisePropertyChanged("IsPasswordDialogOpen");
                }
            }
        }

        private bool _isShowingIntro;
        public bool IsShowingIntro
        {
            get
            {
                return _isShowingIntro;
            }
            set
            {
                if (value != _isShowingIntro)
                {
                    _isShowingIntro = value;
                    RaisePropertyChanged("IsShowingIntro");
                }
            }
        }

        private bool _isPasswordCorrect;
        public bool IsPasswordCorrect
        {
            get
            {
                return _isPasswordCorrect;
            }
            set
            {
                if (value != _isPasswordCorrect)
                {
                    _isPasswordCorrect = value;
                    RaisePropertyChanged("IsPasswordCorrect");
                }
            }
        }

        private string _currentPassword;
        public string CurrentPassword
        {
            get
            {
                return _currentPassword;
            }
            set
            {
                if (value != _currentPassword)
                {
                    _currentPassword = value;
                    RaisePropertyChanged("CurrentPassword");

                    this.IsPasswordCorrect = (this._correctPassword == this.CurrentPassword);
                }
            }
        }

        private bool _isExamsEnabled;
        public bool IsExamsEnabled
        {
            get
            {
                return _isExamsEnabled;
            }
            set
            {
                if (value != _isExamsEnabled)
                {
                    _isExamsEnabled = value;
                    RaisePropertyChanged("IsExamsEnabled");
                }
            }
        }

        private int _availableExams;
        public int AvailableExams
        {
            get
            {
                return _availableExams;
            }
            set
            {
                if (value != _availableExams)
                {
                    _availableExams = value;
                    RaisePropertyChanged("AvailableExams");
                }
            }
        }

        private string _levelMessage;
        public string LevelMessage
        {
            get
            {
                return _levelMessage;
            }
            set
            {
                if (value != _levelMessage)
                {
                    _levelMessage = value;
                    RaisePropertyChanged("LevelMessage");
                }
            }
        }

        private void CheckExams()
        {
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            var examsFolder = appFolder.GetDirectories("Exams");

            this.IsExamsEnabled = examsFolder.Count() > 0 ? true : false;
        }

        private async Task Initialize()
        {
            this.IsLoading = true;

            await Task.Run(async () => {
                this.CheckExams();

                await _speciesDataModel.LoadData();
            });

            this.IsLoading = false;
        }

        private RelayCommand<string> _startWorkshopCommand;
        public RelayCommand<string> StartWorkshopCommand => _startWorkshopCommand
                    ?? (_startWorkshopCommand = new RelayCommand<string>(
                    param =>
                    {
                        this._launchContext = new LaunchContext()
                        {
                            ExerciseType = param.ToExerciseType(),
                            SpeciesType = param.ToSpeciesType()
                        };

                        if (_launchContext.SpeciesType == SpeciesType.FishFamily || _launchContext.SpeciesType == SpeciesType.Coral || this._launchContext.SpeciesType == SpeciesType.Seagrass)
                        {
                            if (_launchContext.ExerciseType == ExerciseType.Exam)
                            {
                                if (!this.IsPasswordCorrect)
                                {
                                    this.IsPasswordDialogOpen = true;
                                }
                                else
                                {
                                    this.ShowExamSelect();
                                }
                            }
                            else
                            {
                                _navigationService.Navigate("/Views/WorkshopPage.xaml", _launchContext);
                            }
                        }
                        else {
                            this.ShowLevelSelect();
                        }
                    }));

        private RelayCommand<string> _showSpeciesBrowserCommand;
        public RelayCommand<string> ShowSpeciesBrowserCommand => _showSpeciesBrowserCommand
                    ?? (_showSpeciesBrowserCommand = new RelayCommand<string>(
                    param =>
                    {
                        _navigationService.Navigate("/Views/SpeciesBrowserPage.xaml", null);
                    }));

        private RelayCommand<string> _chooseLevelCommand;
        public RelayCommand<string> ChooseLevelCommand => _chooseLevelCommand
                    ?? (_chooseLevelCommand = new RelayCommand<string>(
                    param =>
                    {
                        switch (param)
                        {
                            case "Indicator":
                                _launchContext.SurveyLevel = SurveyLevel.Indicator;
                                break;
                            case "Expert":
                                _launchContext.SurveyLevel = SurveyLevel.Expert;
                                break;
                        }

                        this.IsLevelDialogOpen = false;

                        if (_launchContext.ExerciseType == ExerciseType.Exam)
                        {
                            if (!this.IsPasswordCorrect)
                            {
                                this.IsPasswordDialogOpen = true;
                            }
                            else
                            {
                                this.ShowExamSelect();
                            }
                        }
                        else
                        {
                            _navigationService.Navigate("/Views/WorkshopPage.xaml", _launchContext);
                        }
                    }));

        private RelayCommand _closeLevelDialogCommand;
        public RelayCommand CloseLevelDialogCommand => _closeLevelDialogCommand
                    ?? (_closeLevelDialogCommand = new RelayCommand(
                    () =>
                    {                        
                        this.IsLevelDialogOpen = false;
                    }));

        private RelayCommand _closeExamDialogCommand;
        public RelayCommand CloseExamDialogCommand => _closeExamDialogCommand
                    ?? (_closeExamDialogCommand = new RelayCommand(
                    () =>
                    {
                        this.IsExamDialogOpen = false;
                    }));

        private RelayCommand _closePasswordDialogCommand;
        public RelayCommand ClosePasswordDialogCommand => _closePasswordDialogCommand
                    ?? (_closePasswordDialogCommand = new RelayCommand(
                    () =>
                    {
                        this.IsPasswordDialogOpen = false;
                    }));

        private RelayCommand _submitPasswordCommand;
        public RelayCommand SubmitPasswordCommand => _submitPasswordCommand
                    ?? (_submitPasswordCommand = new RelayCommand(
                    () =>
                    {
                        this.IsPasswordDialogOpen = false;

                        if (this.CurrentPassword == this._correctPassword)
                        {
                            this.ShowExamSelect();
                        }
                    }));

        private RelayCommand<string> _chooseExamCommand;
        public RelayCommand<string> ChooseExamCommand => _chooseExamCommand
                    ?? (_chooseExamCommand = new RelayCommand<string>(
                    param =>
                    {
                        switch (param)
                        {
                            case "1":
                                _launchContext.ExamNumber = 1;
                                break;
                            case "2":
                                _launchContext.ExamNumber = 2;
                                break;
                            case "3":
                                _launchContext.ExamNumber = 3;
                                break;
                        }

                        this.IsExamDialogOpen = false;

                        _navigationService.Navigate("/Views/WorkshopPage.xaml", _launchContext);
                    }));

        private void ShowLevelSelect()
        {
            this.LevelMessage = String.Format("Select a UVC level for your {0} {1}.", this._launchContext.SpeciesType.ToFriendlyString(), this._launchContext.ExerciseType.ToString().ToLower());
            this.IsLevelDialogOpen = true;
        }

        private void ShowExamSelect()
        {
            // Check How Many Exams
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            var examsFolder = appFolder.GetDirectories("Exams").First();
            var typeFolder = examsFolder.GetDirectories(this._launchContext.SpeciesType.ToString()).First();
            
            IReadOnlyList<DirectoryInfo> exams;
            if (this._launchContext.SpeciesType == SpeciesType.FishFamily || this._launchContext.SpeciesType == SpeciesType.Coral || this._launchContext.SpeciesType == SpeciesType.Seagrass)
            {
                exams = typeFolder.GetDirectories();
            }
            else
            {
                var levelFolder = typeFolder.GetDirectories(this._launchContext.SurveyLevel.ToString()).First();
                exams = levelFolder.GetDirectories();
            }

            if (exams.Count > 1)
            {
                this.AvailableExams = exams.Count;
                this.IsExamDialogOpen = true;               
            }
            else
            {
                _launchContext.ExamNumber = 1;
                _navigationService.Navigate("/Views/WorkshopPage.xaml", _launchContext);
            }
        }
    }
}