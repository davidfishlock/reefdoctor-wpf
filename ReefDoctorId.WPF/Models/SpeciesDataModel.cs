using ReefDoctorId.Core.Models;
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
                    JuvenileImages = juvenileImagePaths,
                    ImagePath = imagePaths[_random.Next(0, imagePaths.Count - 1)],
                    Info = GetSpeciesInfo(folder),
                    Similar = similarItems
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

                var existingItem = NAItems.FirstOrDefault(item => item.Name == name);

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

            return numberItems > 0 ? NAItems.Take(numberItems).ToList() : NAItems;
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
                var indicatorFolder = speciesTypeFolder.GetDirectories("Indicator").First();
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
                var expertFolder = speciesTypeFolder.GetDirectories("Expert").First();

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
                    var NAFolder = speciesTypeFolder.GetDirectories("NA").FirstOrDefault();

                    if (NAFolder != null)
                    {
                        items.AddRange(FetchNAItems(NAFolder, speciesType));

                        // Add Nudibranchs!
                        if (speciesType == SpeciesType.Invertebrate)
                        {
                            var nudibranchFolder = NAFolder.GetDirectories("Nudibranchs").FirstOrDefault();

                            if (nudibranchFolder != null)
                            {
                                items.AddRange(FetchNAItems(nudibranchFolder, speciesType));
                            }
                        }
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
