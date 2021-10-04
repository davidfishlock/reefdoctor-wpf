using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IndicatorFlipcards.Services;
using Microsoft.Practices.ServiceLocation;
using ReefDoctorId.Core.Models;
using ReefDoctorId.WPF.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReefDoctorId.Core.ViewModels;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using System.Threading;

namespace ReefDoctorId.ViewModels
{
    public class SpeciesBrowserViewModel : BaseViewModel
    {
        private NavigationServiceEx _navigationService;
        private SpeciesDataModel _speciesDataModel;

        public SpeciesBrowserViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            _speciesDataModel = ServiceLocator.Current.GetInstance<SpeciesDataModel>();

            this.CanGoBack = true;

            this.Initialize();
        }

        private SpeciesType StringToSpeciesType(string inputString)
        {
            string speciesType = inputString.Split('-')[0];

            Enum.TryParse(speciesType, out SpeciesType enumValue);
            return enumValue;
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

        private List<Subject> _allSpecies;
        public List<Subject> AllSpecies
        {
            get
            {
                return _allSpecies;
            }
            set
            {
                if (_allSpecies == value)
                {
                    return;
                }

                _allSpecies = value;
                RaisePropertyChanged("AllSpecies");
            }
        }

        private SpeciesType _selectedType = SpeciesType.Fish;
        public SpeciesType SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                if (_selectedType == value)
                {
                    return;
                }

                _selectedType = value;
                RaisePropertyChanged("SelectedType");

                this.ApplyFilter();
            }
        }

        private SurveyLevel _selectedLevel = SurveyLevel.All;
        public SurveyLevel SelectedLevel
        {
            get
            {
                return _selectedLevel;
            }
            set
            {
                if (_selectedLevel == value)
                {
                    return;
                }

                _selectedLevel = value;
                RaisePropertyChanged("SelectedLevel");

                this.ApplyFilter();
            }
        }

        private Subject _selectedItem;
        public Subject SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                {
                    return;
                }

                _selectedItem = value;


                if (value != null)
                {
                    this.MoreImages = this.SelectedItem.Images.Except(new List<string>() { this.SelectedItem.Images[0] }).ToList();
                    this.SelectedImage = this.SelectedItem.Images != null && this.SelectedItem.Images.Count > 0 ? this.SelectedItem.Images[0] : null;
                }

                RaisePropertyChanged("SelectedItem");
            }
        }

        private List<string> _moreImages;
        public List<string> MoreImages
        {
            get
            {
                return _moreImages;
            }
            set
            {
                if (_moreImages == value)
                {
                    return;
                }

                _moreImages = value;

                RaisePropertyChanged("MoreImages");
            }
        }

        private string _selectedImage;
        public string SelectedImage
        {
            get
            {
                return _selectedImage;
            }
            set
            {
                if (_selectedImage == value)
                {
                    return;
                }

                _selectedImage = value;
                RaisePropertyChanged("SelectedImage");
            }
        }

        private bool _isLevelSelectVisible = true;
        public bool IsLevelSelectVisible
        {
            get
            {
                return _isLevelSelectVisible;
            }
            set
            {
                if (_isLevelSelectVisible == value)
                {
                    return;
                }

                _isLevelSelectVisible = value;
                RaisePropertyChanged("IsLevelSelectVisible");
            }
        }

        private bool _isFullScreenVisible;
        public bool IsFullScreenVisible
        {
            get
            {
                return _isFullScreenVisible;
            }
            set
            {
                if (_isFullScreenVisible == value)
                {
                    return;
                }

                _isFullScreenVisible = value;
                RaisePropertyChanged("IsFullScreenVisible");
                RaisePropertyChanged("IsFlipEnabled");
            }
        }

        public bool IsFlipEnabled
        {
            get
            {
                return _isFullScreenVisible;
            }
        }


        private List<SpeciesType> _speciesSelections = new List<SpeciesType> { SpeciesType.Fish, SpeciesType.FishFamily, SpeciesType.Invert, SpeciesType.Benthic, SpeciesType.Coral, SpeciesType.Seagrass };
        public List<SpeciesType> SpeciesSelections
        {
            get
            {
                return _speciesSelections;
            }
            set
            {
                if (_speciesSelections == value)
                {
                    return;
                }

                _speciesSelections = value;
                RaisePropertyChanged("SpeciesSelections");
            }
        }

        private List<SurveyLevel> _levelSelections = new List<SurveyLevel> { SurveyLevel.All, SurveyLevel.Indicator, SurveyLevel.Expert };
        public List<SurveyLevel> LevelSelections
        {
            get
            {
                return _levelSelections;
            }
            set
            {
                if (_levelSelections == value)
                {
                    return;
                }

                _levelSelections = value;
                RaisePropertyChanged("LevelSelections");
            }
        }

        public List<Subject> _currentFilter;
        public List<Subject> CurrentFilter
        {
            get
            {
                return _currentFilter;
            }
            set
            {
                if (_currentFilter == value)
                {
                    return;
                }

                _currentFilter = value;
                RaisePropertyChanged("CurrentFilter");
            }
        }

        private async Task Initialize()
        {
            this.IsLoading = true;

            this.AllSpecies = _speciesDataModel.SpeciesData;
            this.ApplyFilter();

            this.IsLoading = false;
        }

        private void ApplyFilter()
        {
            this.SelectedItem = null;

            if (this.SelectedType == SpeciesType.Benthic)
            {
                this.LevelSelections = new List<SurveyLevel> { SurveyLevel.Indicator, SurveyLevel.Expert };
                if (this.SelectedLevel == SurveyLevel.All)
                {
                    this.SelectedLevel = SurveyLevel.Indicator;
                } 
            }
            else
            {
                this.LevelSelections = new List<SurveyLevel> { SurveyLevel.All, SurveyLevel.Indicator, SurveyLevel.Expert };
            }

            if (this.SelectedType == SpeciesType.FishFamily || this.SelectedType == SpeciesType.Coral || this.SelectedType == SpeciesType.Seagrass)
            {
                this.IsLevelSelectVisible = false; 

                this.CurrentFilter = AllSpecies.Where(
                    item => 
                    item.SpeciesType == this.SelectedType)
                    .OrderBy(item => item.Name)
                    .ToList();
            }
            else
            {
                this.IsLevelSelectVisible = true;

                this.CurrentFilter = AllSpecies.Where(
                    item =>
                    (this.SelectedType == SpeciesType.All 
                    || item.SpeciesType == this.SelectedType) 
                    && (this.SelectedLevel == SurveyLevel.All 
                    || item.SurveyLevel == this.SelectedLevel) 
                    && !item.IsNA)
                    .OrderBy(item => item.Name)
                    .ToList();
            }
            
            this.SelectedItem = this.CurrentFilter[0];
        }

        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand => _goBackCommand
                    ?? (_goBackCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.GoBack();
                    }));

        private RelayCommand<object> _showImageCommand;
        public RelayCommand<object> ShowImageCommand => _showImageCommand
                    ?? (_showImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        this.IsFullScreenVisible = true;
                    }));

        private RelayCommand<object> _hideImageCommand;
        public RelayCommand<object> HideImageCommand => _hideImageCommand
                    ?? (_hideImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        this.IsFullScreenVisible = false;
                    }));

        private RelayCommand<object> _toggleImageCommand;
        public RelayCommand<object> ToggleImageCommand => _toggleImageCommand
                    ?? (_toggleImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        this.IsFullScreenVisible = !this.IsFullScreenVisible;
                    }));

        private RelayCommand<object> _selectImageCommand;
        public RelayCommand<object> SelectImageCommand => _selectImageCommand
                    ?? (_selectImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        this.SelectedImage = (string)param;

                        DispatcherHelper.RunAsync(() => {
                            Thread.Sleep(200);
                            this.IsFullScreenVisible = true;
                        });
                    }));
    }
}