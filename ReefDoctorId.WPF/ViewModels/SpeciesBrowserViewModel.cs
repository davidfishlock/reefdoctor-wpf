using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using ReefDoctorId.Core.Models;
using ReefDoctorId.WPF.Models;
using System.Linq;
using System.Collections.Generic;
using ReefDoctorId.Core.ViewModels;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ReefDoctorId.ViewModels
{
    public class SpeciesBrowserViewModel : BaseViewModel
    {
        private readonly SpeciesDataModel _speciesDataModel;

        public SpeciesBrowserViewModel()
        {
            _speciesDataModel = ServiceLocator.Current.GetInstance<SpeciesDataModel>();

            CanGoBack = true;

            Initialize();
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

                ApplyFilter();
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

                ApplyFilter();
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
                    MoreImages = SelectedItem.Images.Except(new List<string>() { SelectedItem.Images[0] }).ToList();
                    SelectedImage = SelectedItem.Images != null && SelectedItem.Images.Count > 0 ? SelectedItem.Images[0] : null;
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

        private void Initialize()
        {
            IsLoading = true;

            AllSpecies = _speciesDataModel.SpeciesData;
            ApplyFilter();

            IsLoading = false;
        }

        private void ApplyFilter()
        {
            SelectedItem = null;

            if (SelectedType == SpeciesType.Benthic)
            {
                LevelSelections = new List<SurveyLevel> { SurveyLevel.Indicator, SurveyLevel.Expert };
                if (SelectedLevel == SurveyLevel.All)
                {
                    SelectedLevel = SurveyLevel.Indicator;
                }
            }
            else
            {
                LevelSelections = new List<SurveyLevel> { SurveyLevel.All, SurveyLevel.Indicator, SurveyLevel.Expert };
            }

            if (SelectedType == SpeciesType.FishFamily || SelectedType == SpeciesType.Coral || SelectedType == SpeciesType.Seagrass)
            {
                IsLevelSelectVisible = false;

                CurrentFilter = AllSpecies.Where(
                    item =>
                    item.SpeciesType == SelectedType)
                    .OrderBy(item => item.Name)
                    .ToList();
            }
            else
            {
                IsLevelSelectVisible = true;

                CurrentFilter = AllSpecies.Where(
                    item =>
                    (SelectedType == SpeciesType.All
                    || item.SpeciesType == SelectedType)
                    && (SelectedLevel == SurveyLevel.All
                    || item.SurveyLevel == SelectedLevel)
                    && !item.IsNA)
                    .OrderBy(item => item.Name)
                    .ToList();
            }

            SelectedItem = CurrentFilter[0];
        }

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand => _exportCommand
                    ?? (_exportCommand = new RelayCommand(Export));

        private RelayCommand<object> _showImageCommand;
        public RelayCommand<object> ShowImageCommand => _showImageCommand
                    ?? (_showImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        IsFullScreenVisible = true;
                    }));

        private RelayCommand<object> _hideImageCommand;
        public RelayCommand<object> HideImageCommand => _hideImageCommand
                    ?? (_hideImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        IsFullScreenVisible = false;
                    }));

        private RelayCommand<object> _toggleImageCommand;
        public RelayCommand<object> ToggleImageCommand => _toggleImageCommand
                    ?? (_toggleImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        IsFullScreenVisible = !IsFullScreenVisible;
                    }));

        private RelayCommand<object> _selectImageCommand;
        public RelayCommand<object> SelectImageCommand => _selectImageCommand
                    ?? (_selectImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        SelectedImage = (string)param;

                        DispatcherHelper.RunAsync(() =>
                        {
                            Thread.Sleep(200);
                            IsFullScreenVisible = true;
                        });
                    }));

        private void Export()
        {
            var csv = new StringBuilder();
            var rowId = 1;

            foreach (var species in _speciesDataModel.SpeciesData)
            {
                if (species.IsNA) continue;

                var info = new StringBuilder();
                var lineCount = 1;

                if (species.Info != null)
                {
                    foreach (var line in species.Info)
                    {
                        if (lineCount > 1) info.Append("~");
                        info.Append(line);
                        lineCount++;
                    }
                }

                var entry = string.Format("{0},{1},{2},{3},{4},{5}", rowId, species.Name, toDBFormat(species.SpeciesType), species.SurveyLevel, species.Images.Count, string.Format("\"{0}\"", info.ToString()));
                csv.AppendLine(entry);
                rowId++;
            }

            File.WriteAllText("C://Users/David/Desktop/test.csv", csv.ToString());
        }

        private string toDBFormat(SpeciesType type)
        {
            switch (type)
            {
                case SpeciesType.Invert:
                    return "Invertebrate";
                default:
                    return type.ToString();
            }
        }
    }
}