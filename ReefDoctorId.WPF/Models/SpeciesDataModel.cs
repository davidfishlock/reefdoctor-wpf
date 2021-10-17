﻿using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using ReefDoctorId.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReefDoctorId.WPF.Models
{
    public class SpeciesDataModel
    {
        private List<string> _indicatorBenthicStrings = new List<string>();
        private List<string> _expertBenthicStrings = new List<string>();
        private List<string> _seagrassStrings = new List<string>();

        private Random _random = new Random();

        public List<Subject> SpeciesData { get; set; }

        private void LoadCodeStrings()
        {
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            var dataFolder = appFolder.GetDirectories("Species Data").First();
            var benthicFolder = dataFolder.GetDirectories("Benthic").First();
            var seagrassFolder = dataFolder.GetDirectories("Seagrass").First();

            var indicatorBenthicFile = benthicFolder.GetFiles("IndicatorCodes.txt").FirstOrDefault();
            var expertBenthicFile = benthicFolder.GetFiles("ExpertCodes.txt").FirstOrDefault();
            var seagrassFile = seagrassFolder.GetFiles("Codes.txt").FirstOrDefault();

            if (indicatorBenthicFile != null)
            {
                using (var indicatorStream = indicatorBenthicFile.OpenRead())
                {
                    using (var streamReader = new StreamReader(indicatorStream))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            _indicatorBenthicStrings.Add(streamReader.ReadLine());
                        }
                    }
                }
            }

            if (expertBenthicFile != null)
            {
                using (var expertStream = expertBenthicFile.OpenRead())
                {
                    using (var streamReader = new StreamReader(expertStream))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            _expertBenthicStrings.Add(streamReader.ReadLine());
                        }
                    }
                }
            }

            if (seagrassFile != null)
            {
                using (var seagrassStream = seagrassFile.OpenRead())
                {
                    using (var streamReader = new StreamReader(seagrassStream))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            _seagrassStrings.Add(streamReader.ReadLine());
                        }
                    }
                }
            }
        }

        private List<string> GetSpeciesInfo(DirectoryInfo folder)
        {
            var infoFile = folder.GetFiles("Info.txt").FirstOrDefault();

            List<string> infoItems = new List<string>();

            if (infoFile != null)
            {
                using (var infoStream = infoFile.OpenRead())
                {
                    using (var streamReader = new StreamReader(infoStream))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            infoItems.Add(streamReader.ReadLine());
                        }
                    }
                }
            }

            return infoItems;
        }

        private Subject CreateSubject(DirectoryInfo folder)
        {
            var images = folder.GetFiles().Where(file => Extensions.IsImage(file.Extension)).ToList();

            if (images.Count > 0)
            {
                var imagePaths = new List<string>();

                foreach (var image in images)
                {
                    imagePaths.Add(image.FullName);
                }

                var speciesInfo = GetSpeciesInfo(folder);
#if DEBUG
                if (speciesInfo == null)
                {
                    Debug.WriteLine("Data Issues: No Species Info for: " + folder.Name);
                }
#endif      
                return new Subject
                {
                    Name = folder.Name,
                    Images = imagePaths,
                    ImagePath = imagePaths[_random.Next(0, imagePaths.Count - 1)],
                    Info = GetSpeciesInfo(folder),
                };
            }
#if DEBUG
            else
            {
                Debug.WriteLine("Data Issues: No Species Images for: " + folder.Name);
                return null;
            }
#endif
        }

        private List<Subject> LoadSpeciesData(SpeciesType speciesType)
        {
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            List<Subject> items = new List<Subject>();

            var dataFolder = appFolder.GetDirectories("Species Data").First();
            var speciesTypeFolder = dataFolder.GetDirectories(speciesType.ToString()).First();

            if (speciesType != SpeciesType.FishFamily && speciesType != SpeciesType.Coral && speciesType != SpeciesType.Seagrass)
            {
                // Take Indicator Species
                var indicatorFolder = speciesTypeFolder.GetDirectories("Indicator").FirstOrDefault();
                foreach (var folder in indicatorFolder.GetDirectories())
                {
                    var subject = CreateSubject(folder);
                    if (subject != null)
                    {
                        subject.SurveyLevel = SurveyLevel.Indicator;
                        subject.SpeciesType = speciesType;
                        items.Add(subject);
                    }
                    else
                    {
                        Debug.WriteLine("Data Issues: Failed to create model for: " + folder.Name);
                    }
                }

                // Take Expert Species
                var expertFolder = speciesTypeFolder.GetDirectories("Expert").FirstOrDefault();

                foreach (var folder in expertFolder.GetDirectories())
                {
                    var subject = CreateSubject(folder);
                    if (subject != null)
                    {
                        subject.SurveyLevel = SurveyLevel.Expert;
                        subject.SpeciesType = speciesType;
                        items.Add(subject);
                    }
                    else
                    {
                        Debug.WriteLine("Data Issues: Failed to create model for: " + folder.Name);
                    }
                }

                if (speciesType == SpeciesType.Fish || speciesType == SpeciesType.Invertebrate)
                {
                    // Take NA Species
                    var naFolder = speciesTypeFolder.GetDirectories("NA").FirstOrDefault();

                    foreach (var folder in naFolder.GetDirectories())
                    {
                        var subject = CreateSubject(folder);
                        if (subject != null)
                        {
                            subject.SurveyLevel = SurveyLevel.NA;
                            subject.SpeciesType = speciesType;
                            items.Add(subject);
                        }
                        else
                        {
                            Debug.WriteLine("Data Issues: Failed to create model for: " + folder.Name);
                        }
                    }
                }

                if (speciesType == SpeciesType.Benthic)
                {
                    // Apply display strings for Benthic
                    foreach (var benthicCode in items)
                    {
                        benthicCode.Name = benthicCode.SurveyLevel == SurveyLevel.Indicator ?
                            _indicatorBenthicStrings.Where(item => item.EndsWith(" " + benthicCode.Name)).ToList().FirstOrDefault() :
                            _expertBenthicStrings.Where(item => item.EndsWith(" " + benthicCode.Name)).ToList().FirstOrDefault();
                    }
                }
            }
            else if (speciesType == SpeciesType.FishFamily || speciesType == SpeciesType.Coral || speciesType == SpeciesType.Seagrass)
            {
                foreach (var folder in speciesTypeFolder.GetDirectories())
                {
                    var subject = CreateSubject(folder);
                    if (subject != null)
                    {
                        subject.SurveyLevel = SurveyLevel.Indicator;
                        subject.SpeciesType = speciesType;
                        items.Add(subject);
                    }
                    else
                    {
                        Debug.WriteLine("Data Issues: Failed to create model for: " + folder.Name);
                    }
                }

                if (speciesType == SpeciesType.Seagrass)
                {
                    // Apply display strings for Seagrass
                    foreach (var seagrassCode in items)
                    {
                        seagrassCode.Name = _seagrassStrings.Where(item => item.EndsWith(" " + seagrassCode.Name)).ToList().FirstOrDefault();
                    }
                }
            }

            return items;
        }

        public void LoadData()
        {
            // Load code string mappings
            LoadCodeStrings();

            // Load all species data
            SpeciesData = new List<Subject>();
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.Fish));
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.Invertebrate));
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.Benthic));
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.FishFamily));
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.Coral));
            SpeciesData.AddRange(LoadSpeciesData(SpeciesType.Seagrass));

#if DEBUG
            SpeciesData = SpeciesData.OrderBy(subject => subject.Images.Count).ToList();
            foreach (var subject in SpeciesData.Where(subject => subject.SpeciesType == SpeciesType.Fish))
            {
                Debug.WriteLine(subject.Name + ": " + subject.Images.Count + " images, " + (subject.Info != null && subject.Info.Count > 0) + " info data");
            }
#endif
        }
    }
}
