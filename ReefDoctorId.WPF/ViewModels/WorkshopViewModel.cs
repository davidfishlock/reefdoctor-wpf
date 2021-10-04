using GalaSoft.MvvmLight.Command;
using IndicatorFlipcards.Services;
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
        private NavigationServiceEx _navigationService;
        private SpeciesDataModel _speciesDataModel;

        private Random _random = new Random();

        private const int NumberOfNA = 10;

        public WorkshopViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<NavigationServiceEx>();
            _speciesDataModel = ServiceLocator.Current.GetInstance<SpeciesDataModel>();

            this.CanGoBack = true;
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

                this.SelectedItemImages = _isInfoVisible ? this.SelectedItem.Images : new List<string>();

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
                this.SelectedItemImages = _isFullScreenVisible || IsInfoVisible ? this.SelectedItem.Images : new List<string>();

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

                this.IsCompleted = _progress == _speciesItems.Count - 1;
                
                RaisePropertyChanged("Progress");
            }
        }
        
        private LaunchContext _launchContext;
        public LaunchContext LaunchContext {
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

                    this.IsLoading = true;
                    this.Initialize();
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

                    this.Progress = this.SpeciesItems.IndexOf(this.SelectedItem);
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

                this.SelectedIndex = 0;
                this.SelectedItem = SpeciesItems.Count > 0 ? SpeciesItems[0] : null;
            }
        }
        
        private List<Subject> FetchExamItems(DirectoryInfo folder)
        {
            var files = folder.GetFiles();
            var examFiles = files.Where(file => Extensions.IsImage(file.Extension)).OrderBy(file => Int32.Parse(file.Name.Split(' ')[0])).ToList();
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
            this.SpeciesItems = new List<Subject>();

            await Task.Run(() => {
                this.IsWorkshop = this.LaunchContext.ExerciseType == ExerciseType.Workshop;
                this.IsExam = this.LaunchContext.ExerciseType == ExerciseType.Exam;

                List<Subject> items = new List<Subject>();

                if (this.IsExam)
                {
                    var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;

                    var examsFolder = appFolder.GetDirectories("Exams").First();
                    var typeFolder = examsFolder.GetDirectories(this._launchContext.SpeciesType.ToString()).First();
                    DirectoryInfo levelFolder = this._launchContext.SpeciesType == SpeciesType.FishFamily || this._launchContext.SpeciesType == SpeciesType.Coral || this._launchContext.SpeciesType == SpeciesType.Seagrass ? typeFolder : typeFolder.GetDirectories(this._launchContext.SurveyLevel.ToString()).First();
                    var examFolder = levelFolder.GetDirectories(this._launchContext.ExamNumber.ToString()).First();

                    items.AddRange(this.FetchExamItems(examFolder));
                }
                else
                {
                    if (this.LaunchContext.SpeciesType == SpeciesType.Benthic)
                    {
                        List<Subject> benthicItems = new List<Subject>();
                        int numberItems = 0;

                        if (this.LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                        {
                            numberItems = 3;
                            benthicItems.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == this._launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Indicator && !item.IsNA));
                        }
                        else if (this.LaunchContext.SurveyLevel == SurveyLevel.Expert)
                        {
                            numberItems = 2;
                            benthicItems.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == this._launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Expert && !item.IsNA));
                        }

                        foreach (var item in benthicItems)
                        {
                            item.Images.Shuffle();
                            var subItems = item.Images.Take(numberItems);
                            
                            // Add up to 3 items for each code
                            foreach (var subItem in subItems)
                            {
                                items.Add(
                                    new Subject() {
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
                        if (this.LaunchContext.SpeciesType == SpeciesType.JuvenileFish)
                        {
                            // Fetch Fish with Juvenile info
                            if (this.LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == SpeciesType.Fish && item.SurveyLevel == this.LaunchContext.SurveyLevel && item.JuvenileImages != null && item.JuvenileImages.Count > 0));
                            }
                            else if (this.LaunchContext.SurveyLevel == SurveyLevel.Expert)
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
                            if (this.LaunchContext.SurveyLevel == SurveyLevel.Indicator)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == this._launchContext.SpeciesType && item.SurveyLevel == SurveyLevel.Indicator && !item.IsNA));
                            }
                            else if (this.LaunchContext.SurveyLevel == SurveyLevel.Expert)
                            {
                                items.AddRange(_speciesDataModel.SpeciesData.Where(item => item.SpeciesType == this._launchContext.SpeciesType && !item.IsNA));
                            }

                            this.SelectImages(items);
                        }
                    }

                    // Fetch NA Items
                    if (this.LaunchContext.SpeciesType == SpeciesType.Fish || this.LaunchContext.SpeciesType == SpeciesType.Invert)
                    {
                        var NAItems = _speciesDataModel.SpeciesData.Where(item => item.SpeciesType == this._launchContext.SpeciesType && item.IsNA).ToList();
                        NAItems.Shuffle();
                        NAItems = NAItems.Take(Convert.ToInt32(items.Count / 4)).ToList();
                        items.AddRange(NAItems);
                    }

                    // Shuffle the items
                    items.Shuffle();
                }

                this.AssignIndexes(items);

                // Add dud completion item
                items.Add(new Subject());
                this.SpeciesItems = items;

                this.SelectedIndex = 0;
                this.SelectedItem = this.SpeciesItems[0];

                this.IsLoading = false;
            });
        }

        public void TearDown()
        {
            this.IsCompleted = false;
            this.SelectedItem = null;
            this.SelectedIndex = 0;
            this.SpeciesItems = new List<Subject>();
            this.IsLoading = true;
        }

        private RelayCommand _showNameCommand;
        public RelayCommand ShowNameCommand => _showNameCommand
                    ?? (_showNameCommand = new RelayCommand(
                    () =>
                    {
                        if (this.SelectedItem != null)
                        {
                            this.SelectedItem.IsNameVisible = true;
                        }
                    }));

        private RelayCommand _hideNameCommand;
        public RelayCommand HideNameCommand => _hideNameCommand
                    ?? (_hideNameCommand = new RelayCommand(
                    () =>
                    {
                        if (this.SelectedItem != null)
                        {
                            this.SelectedItem.IsNameVisible = false;
                        }
                    }));

        private RelayCommand _showInfoCommand;
        public RelayCommand ShowInfoCommand => _showInfoCommand
                    ?? (_showInfoCommand = new RelayCommand(
                    () =>
                    {
                        this.IsInfoVisible = true;
                    }));

        private RelayCommand _hideInfoCommand;
        public RelayCommand HideInfoCommand => _hideInfoCommand
                    ?? (_hideInfoCommand = new RelayCommand(
                    () =>
                    {
                        this.IsInfoVisible = false;
                    }));

        private RelayCommand _hideImageCommand;
        public RelayCommand HideImageCommand => _hideImageCommand
                    ?? (_hideImageCommand = new RelayCommand(
                    () =>
                    {
                        this.IsFullScreenVisible = false;
                    }));

        private RelayCommand _goBackCommand;
        public RelayCommand GoBackCommand => _goBackCommand
                    ?? (_goBackCommand = new RelayCommand(
                    () =>
                    {
                        SpeciesItems = new List<Subject>();
                        this.TearDown();
                        _navigationService.GoBack();
                    }));

        private RelayCommand<object> _showImageCommand;
        public RelayCommand<object> ShowImageCommand => _showImageCommand
                    ?? (_showImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        if (param is string)
                        {
                            this.SelectedImage = (string)param;
                        }
                        else if ((Subject)param != null)
                        {
                            this.SelectedImage = ((Subject)param).ImagePath;
                        }
                        
                        this.IsFullScreenVisible = true;
                    }));

        private RelayCommand<object> _toggleImageCommand;
        public RelayCommand<object> ToggleImageCommand => _toggleImageCommand
                    ?? (_toggleImageCommand = new RelayCommand<object>(
                    param =>
                    {
                        if (param is string)
                        {
                            this.SelectedImage = (string)param;
                        }
                        else if ((Subject)param != null)
                        {
                            this.SelectedImage = ((Subject)param).ImagePath;
                        }

                        this.IsFullScreenVisible = !this.IsFullScreenVisible;
                    }));
    }
}