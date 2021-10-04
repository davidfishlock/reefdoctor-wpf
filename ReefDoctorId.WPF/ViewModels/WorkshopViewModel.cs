using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using ReefDoctorId.UWP;
using ReefDoctorId.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReefDoctorId.ViewModels
{
    public class WorkshopViewModel : BaseViewModel
    {
        private SpeciesDataModel _speciesDataModel;

        private Random _random = new Random();

        public WorkshopViewModel()
        {
            _speciesDataModel = ServiceLocator.Current.GetInstance<SpeciesDataModel>();

            CanGoBack = true;
        }

        private bool _isWorkshop;
        public bool IsWorkshop
        {
            get
            {
                return _isWorkshop;
            }
            set
            {
                if (_isWorkshop == value)
                {
                    return;
                }

                _isWorkshop = value;
                RaisePropertyChanged("IsWorkshop");
            }
        }

        private bool _isExam;
        public bool IsExam
        {
            get
            {
                return _isExam;
            }
            set
            {
                if (_isExam == value)
                {
                    return;
                }

                _isExam = value;
                RaisePropertyChanged("IsExam");
            }
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

        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get
            {
                return _isInfoVisible;
            }
            set
            {
                if (_isInfoVisible == value)
                {
                    return;
                }

                _isInfoVisible = value;

                SelectedItemImages = _isInfoVisible ? SelectedItem.Images : new List<string>();

                RaisePropertyChanged("IsInfoVisible");
                RaisePropertyChanged("IsFlipEnabled");
            }
        }

        public bool IsFlipEnabled
        {
            get
            {
                return _isInfoVisible;
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
                SelectedItemImages = _isFullScreenVisible || IsInfoVisible ? SelectedItem.Images : new List<string>();

                RaisePropertyChanged("IsFullScreenVisible");
            }
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                if (_isCompleted == value)
                {
                    return;
                }

                _isCompleted = value;
                RaisePropertyChanged("IsCompleted");
            }
        }

        private int _progress;
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress == value)
                {
                    return;
                }

                _progress = value;

                IsCompleted = _progress == _speciesItems.Count - 1;

                RaisePropertyChanged("Progress");
            }
        }

        private LaunchContext _launchContext;
        public LaunchContext LaunchContext
        {
            get
            {
                return _launchContext;
            }
            set
            {
                if (value != _launchContext)
                {
                    _launchContext = value;
                    RaisePropertyChanged("LaunchContext");

                    IsLoading = true;
                    Initialize();
                }
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (value != _selectedIndex)
                {
                    _selectedIndex = value;
                    RaisePropertyChanged("SelectedIndex");
                }
            }
        }

        private List<string> _selectedItemImages = new List<string>();
        public List<string> SelectedItemImages
        {
            get
            {
                return _selectedItemImages;
            }
            set
            {
                if (value != _selectedItemImages)
                {
                    _selectedItemImages = value;
                    RaisePropertyChanged("SelectedItemImages");
                }
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
                if (value != _selectedItem)
                {
                    if (_selectedItem != null)
                    {
                        _selectedItem.IsNameVisible = false;
                    }

                    _selectedItem = value;
                    RaisePropertyChanged("SelectedItem");

                    Progress = SpeciesItems.IndexOf(SelectedItem);
                }
            }
        }

        private string _selectedImage = null;
        public string SelectedImage
        {
            get
            {
                return _selectedImage;
            }
            set
            {
                if (value != _selectedImage)
                {
                    _selectedImage = value;
                    RaisePropertyChanged("SelectedImage");
                }
            }
        }

        private List<Subject> _speciesItems;
        public List<Subject> SpeciesItems
        {
            get
            {
                return _speciesItems;
            }

            set
            {
                if (_speciesItems == value)
                {
                    return;
                }

                _speciesItems = value;
                RaisePropertyChanged("SpeciesItems");

                SelectedIndex = 0;
                SelectedItem = SpeciesItems.Count > 0 ? SpeciesItems[0] : null;
            }
        }

        private List<Subject> FetchExamItems(DirectoryInfo folder)
        {
            var files = folder.GetFiles();
            var examFiles = files.Where(file => Extensions.IsImage(file.Extension)).OrderBy(file => int.Parse(file.Name.Split(' ')[0])).ToList();
            var examItems = new List<Subject>();

            foreach (var file in examFiles)
            {
                bool isNA = false;

                var name = file.Name.Split('.').First();

                if (name.Contains("NA"))
                {
                    name = name.Replace("NA - ", "");
                    isNA = true;
                }

                var firstDivider = name.IndexOf("- ");
                name = name.Substring(firstDivider + 2);

                examItems.Add(new Subject
                {
                    Name = name,
                    Images = new List<string>() { file.FullName },
                    ImagePath = file.FullName,
                    IsNA = isNA
                });
            }

            return examItems;
        }

        private void SelectImages(List<Subject> list)
        {
            foreach (var item in list)
            {
                item.ImagePath = item.Images[_random.Next(0, item.Images.Count - 1)];
            }
        }

        private void AssignIndexes(List<Subject> list)
        {
            var index = 1;
            foreach (var item in list)
            {
                item.Index = index;
                index++;
            }
        }

        private async Task Initialize()
        {
            SpeciesItems = new List<Subject>();

            await Task.Run(() =>
            {
                IsWorkshop = LaunchContext.ExerciseType == ExerciseType.Workshop;
                IsExam = LaunchContext.ExerciseType == ExerciseType.Exam;

                List<Subject> items = new List<Subject>();

                if (IsExam)
                {
                    var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;

                    var examsFolder = appFolder.GetDirectories("Exams").First();
                    var typeFolder = examsFolder.GetDirectories(_launchContext.SpeciesType.ToString()).First();
                    var levelFolder = _launchContext.SpeciesType == SpeciesType.FishFamily || _launchContext.SpeciesType == SpeciesType.Coral || _launchContext.SpeciesType == SpeciesType.Seagrass ? typeFolder : typeFolder.GetDirectories(_launchContext.SurveyLevel.ToString()).First();
                    var examFolder = levelFolder.GetDirectories(_launchContext.ExamNumber.ToString()).First();

                    items.AddRange(FetchExamItems(examFolder));
                }
                else
                {
                    if (LaunchContext.SpeciesType == SpeciesType.Benthic)
                    {
                        List<Subject> benthicItems = new List<Subject>();
                        int numberItems = 0;

                        if (LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                        {
                            numberItems = 3;
                            benthicItems.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == _launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Indicator && !item.IsNA));
                        }
                        else if (LaunchContext.SurveyLevel == SurveyLevel.Expert)
                        {
                            numberItems = 2;
                            benthicItems.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == _launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Expert && !item.IsNA));
                        }

                        foreach (var item in benthicItems)
                        {
                            item.Images.Shuffle();
                            var subItems = item.Images.Take(numberItems);

                            // Add up to 3 items for each code
                            foreach (var subItem in subItems)
                            {
                                items.Add(
                                    new Subject()
                                    {
                                        Name = item.Name,
                                        Images = new List<string> { subItem },
                                        SpeciesType = item.SpeciesType,
                                        SurveyLevel = item.SurveyLevel,
                                        ImagePath = subItem
                                    });
                            }
                        }
                    }
                    else
                    {
                        if (LaunchContext.SpeciesType == SpeciesType.JuvenileFish)
                        {
                            // Fetch Fish with Juvenile info
                            if (LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == SpeciesType.Fish && item.SurveyLevel == LaunchContext.SurveyLevel && item.JuvenileImages != null && item.JuvenileImages.Count > 0));
                            }
                            else if (LaunchContext.SurveyLevel == SurveyLevel.Expert)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == SpeciesType.Fish && item.JuvenileImages != null && item.JuvenileImages.Count > 0));
                            }

                            foreach (var item in items)
                            {
                                item.ImagePath = item.JuvenileImages[_random.Next(0, item.JuvenileImages.Count - 1)];
                            }
                        }
                        else
                        {
                            // Fetch Items For Configuration
                            if (LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == _launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Indicator && !item.IsNA));
                            }
                            else if (LaunchContext.SurveyLevel == SurveyLevel.Expert)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == _launchContext.SpeciesType && !item.IsNA));
                            }

                            SelectImages(items);
                        }
                    }

                    // Fetch NA Items
                    if (LaunchContext.SpeciesType == SpeciesType.Fish || LaunchContext.SpeciesType == SpeciesType.Invert)
                    {
                        var NAItems = _speciesDataModel.SpeciesData.Where(item => item.SpeciesType == _launchContext.SpeciesType && item.IsNA).ToList();
                        NAItems.Shuffle();
                        NAItems = NAItems.Take(Convert.ToInt32(items.Count / 4)).ToList();
                        items.AddRange(NAItems);
                    }

                    // Shuffle the items
                    items.Shuffle();
                }

                AssignIndexes(items);

                // Add dud completion item
                items.Add(new Subject());
                SpeciesItems = items;

                SelectedIndex = 0;
                SelectedItem = SpeciesItems[0];

                IsLoading = false;
            });
        }

        public void TearDown()
        {
            IsCompleted = false;
            SelectedItem = null;
            SelectedIndex = 0;
            SpeciesItems = new List<Subject>();
            IsLoading = true;
        }

        private RelayCommand _showNameCommand;
        public RelayCommand ShowNameCommand => _showNameCommand
                    ?? (_showNameCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedItem != null)
                        {
                            SelectedItem.IsNameVisible = true;
                        }
                    }));

        private RelayCommand _hideNameCommand;
        public RelayCommand HideNameCommand => _hideNameCommand
                    ?? (_hideNameCommand = new RelayCommand(
                    () =>
                    {
                        if (SelectedItem != null)
                        {
                            SelectedItem.IsNameVisible = false;
                        }
                    }));

        private RelayCommand _showInfoCommand;
        public RelayCommand ShowInfoCommand => _showInfoCommand
                    ?? (_showInfoCommand = new RelayCommand(
                    () =>
                    {
                        IsInfoVisible = true;
                    }));

        private RelayCommand _hideInfoCommand;
        public RelayCommand HideInfoCommand => _hideInfoCommand
                    ?? (_hideInfoCommand = new RelayCommand(
                    () =>
                    {
                        IsInfoVisible = false;
                    }));

        private RelayCommand _hideImageCommand;
        public RelayCommand HideImageCommand => _hideImageCommand
                    ?? (_hideImageCommand = new RelayCommand(
                    () =>
                    {
                        IsFullScreenVisible = false;
                    }));

        private RelayCommand<object> _showImageCommand;
        public RelayCommand<object> ShowImageCommand => _showImageCommand
                    ?? (_showImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        if (param is string)
                        {
                            SelectedImage = (string)param;
                        }
                        else if ((Subject)param != null)
                        {
                            SelectedImage = ((Subject)param).ImagePath;
                        }

                        IsFullScreenVisible = true;
                    }));

        private RelayCommand<object> _toggleImageCommand;
        public RelayCommand<object> ToggleImageCommand => _toggleImageCommand
                    ?? (_toggleImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        if (param is string)
                        {
                            SelectedImage = (string)param;
                        }
                        else if ((Subject)param != null)
                        {
                            SelectedImage = ((Subject)param).ImagePath;
                        }

                        IsFullScreenVisible = !IsFullScreenVisible;
                    }));
    }
}