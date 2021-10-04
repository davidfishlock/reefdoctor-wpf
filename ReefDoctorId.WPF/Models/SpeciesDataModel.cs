﻿using ReefDoctorId.Core.Models;
using ReefDoctorId.Core.ViewModels;
using ReefDoctorId.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            List<Subject> items = new List<Subject>();

            var dataFolder = appFolder.GetDirectories("Species Data").First();
            var benthicFolder = dataFolder.GetDirectories("Benthic").First();
            var seagrassFolder = dataFolder.GetDirectories("Seagrass").First();

            var indicatorBenthicFile = benthicFolder.GetFiles("IndicatorCodes.txt").FirstOrDefault() as FileInfo;
            var expertBenthicFile = benthicFolder.GetFiles("ExpertCodes.txt").FirstOrDefault() as FileInfo;
            var seagrassFile = seagrassFolder.GetFiles("Codes.txt").FirstOrDefault() as FileInfo;

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

        private async Task<List<string>> GetSpeciesInfo(DirectoryInfo folder)
        {
            var infoFile = folder.GetFiles("Info.txt").FirstOrDefault() as FileInfo;

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

        private async Task<Subject> CreateSubject(DirectoryInfo folder)
        {
            var images = folder.GetFiles().Where(file => Extensions.IsImage(file.Extension)).ToList();

            var speciesImages = images.Where(file => !file.Name.Contains("Similar")).ToList();
            var similarImages = images.Where(file => file.Name.Contains("Similar")).ToList();
            var juvenileImages = images.Where(file => file.Name.Contains("Juvenile")).ToList();
            var similarItems = new List<Subject>();

            foreach (var file in similarImages)
            {
                var name = file.Name.Split('.').First();
                name = name.Replace("Similar - ", "");
                similarItems.Add(new Subject() { Name = name, Images = new List<string>() { file.FullName }, ImagePath = file.FullName });
            }

            var juvenileImagePaths = new List<string>();

            if (juvenileImages.Count > 0)
            {
                foreach (var image in juvenileImages)
                {
                    juvenileImagePaths.Add(image.FullName);
                }
            }

            if (speciesImages.Count > 0)
            {
                var imagePaths = new List<string>();

                foreach (var image in speciesImages)
                {
                    imagePaths.Add(image.FullName);
                }

                var speciesInfo = await this.GetSpeciesInfo(folder);

                if (speciesInfo == null)
                {
                    Debug.WriteLine("Data Issues: No Species Info for: " + folder.Name);
                }

                return new Subject
                {
                    Name = folder.Name,
                    Images = imagePaths,
                    JuvenileImages = juvenileImagePaths,
                    ImagePath = imagePaths[_random.Next(0, imagePaths.Count - 1)],
                    Info = await this.GetSpeciesInfo(folder),
                    Similar = similarItems
                };
            }
            else
            {
                Debug.WriteLine("Data Issues: No Species Images for: " + folder.Name);
                return null;
            }
        }

        private List<Subject> FetchNAItems(DirectoryInfo folder, SpeciesType speciesType, int numberItems = 0)
        {
            var files = folder.GetFiles();
            var NAFiles = files.ToList();
            NAFiles.Shuffle();

            var NAItems = new List<Subject>();

            var numbers = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", "(", ")", "_", "-" };

            foreach (var file in NAFiles)
            {
                var name = file.Name.Split('.').First();

                // Strip Numbers from end of name
                if (folder.Name == "NA")
                {
                    while (name.Length > 0)
                    {
                        if (numbers.Any(item => item.Equals(name.Substring(name.Length - 1))))
                        {
                            name = name.Substring(0, name.Length - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (folder.Name == "Nudibranchs")
                {
                    name = "Nudibranch";
                }

                var existingItem = NAItems.Where(item => item.Name == name).FirstOrDefault();

                if (existingItem != null)
                {
                    existingItem.Images.Add(file.FullName);
                    existingItem.ImagePath = existingItem.Images[_random.Next(0, existingItem.Images.Count - 1)];
                }
                else
                {
                    NAItems.Add(new Subject
                    {
                        Name = name,
                        SurveyLevel = SurveyLevel.NA,
                        SpeciesType = speciesType,
                        IsNA = true,
                        Images = new List<string>() { file.FullName },
                        ImagePath = file.FullName
                    });
                }
            }

            if (numberItems != 0)
            {
                return NAItems.Take(numberItems).ToList();
            }
            else
            {
                return NAItems;
            }
        }

        private async Task<List<Subject>> LoadSpeciesData(SpeciesType speciesType)
        {
            var appFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent;
            List<Subject> items = new List<Subject>();

            var dataFolder = appFolder.GetDirectories("Species Data").First();
            var speciesTypeFolder = dataFolder.GetDirectories(speciesType.ToString()).First();

            if (speciesType != SpeciesType.FishFamily && speciesType != SpeciesType.Coral && speciesType != SpeciesType.Seagrass)
            {
                // Take Indicator Species
                var indicatorFolder = speciesTypeFolder.GetDirectories("Indicator").First();
                foreach (var folder in indicatorFolder.GetDirectories())
                {
                    var subject = await this.CreateSubject(folder);
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
                var expertFolder = speciesTypeFolder.GetDirectories("Expert").First();

                foreach (var folder in expertFolder.GetDirectories())
                {
                    var subject = await this.CreateSubject(folder);
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
                else
                {
                    // Fetch NA Items
                    var NAFolder = speciesTypeFolder.GetDirectories("NA").First();
                    items.AddRange(this.FetchNAItems(NAFolder, speciesType));

                    // Add Nudibranchs!
                    if (speciesType == SpeciesType.Invert)
                    {
                        var nudibranchFolder = NAFolder.GetDirectories("Nudibranchs").First();
                        items.AddRange(this.FetchNAItems(nudibranchFolder, speciesType));
                    }
                }
            }
            else if (speciesType == SpeciesType.FishFamily || speciesType == SpeciesType.Coral || speciesType == SpeciesType.Seagrass)
            {
                foreach (var folder in speciesTypeFolder.GetDirectories())
                {
                    var subject = await this.CreateSubject(folder);
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

        public async Task LoadData()
        {
            // Load code string mappings
            this.LoadCodeStrings();

            // Load all species data
            this.SpeciesData = new List<Subject>();
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.Fish));
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.Invert));
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.Benthic));
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.FishFamily));
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.Coral));
            this.SpeciesData.AddRange(await this.LoadSpeciesData(SpeciesType.Seagrass));

#if DEBUG
            this.SpeciesData = this.SpeciesData.OrderBy(subject => subject.Images.Count).ToList();
            foreach (var subject in this.SpeciesData.Where(subject => subject.SpeciesType == SpeciesType.Fish))
            {
                Debug.WriteLine(subject.Name + ": " + subject.Images.Count + " images, " + (subject.Info != null && subject.Info.Count > 0) + " info data");
            }
#endif
        }
    }
}
