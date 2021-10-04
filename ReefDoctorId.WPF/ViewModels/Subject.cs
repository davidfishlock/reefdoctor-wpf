using GalaSoft.MvvmLight;
using ReefDoctorId.Core.Models;
using ReefDoctorId.ViewModels;
using System.Collections.Generic;

namespace ReefDoctorId.Core.ViewModels
{
    public class Subject : BaseViewModel
    {
        private bool _isNameVisible;
        public bool IsNameVisible
        {
            get
            {
                 return _isNameVisible;
            }
            set
            {
                if (_isNameVisible == value)
                {
                    return;
                }

                _isNameVisible = value;
                RaisePropertyChanged("IsNameVisible");
            }
        }

        public int Index { get; set; }
        public SpeciesType SpeciesType { get; set; }
        public SurveyLevel SurveyLevel { get; set; }
        public string Name { get; set; }
        public bool IsNA { get; set; }
        public List<string> Images { get; set; }
        public List<string> JuvenileImages { get; set; }
        public string ImagePath { get; set; }
        public List<string> Info { get; set; }
        public List<Subject> Similar { get; set; }
    }
}
