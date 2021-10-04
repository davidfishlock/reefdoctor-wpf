using ReefDoctorId.Core.Models;
using System;
using System.Collections.Generic;

namespace ReefDoctorId.UWP
{
    public static class Extensions
    {
        public static string ToFriendlyString(this SpeciesType type)
        {
            switch (type)
            {
                case SpeciesType.FishFamily:
                    return "Fish Families";
                default:
                    return type.ToString();
            }
        }

        public static bool IsImage(string fileExtension)
        {
            var supportedTypes = new List<string> { ".jpg", ".jpeg", ".tif", ".tiff", ".bmp", ".png" };

            return supportedTypes.Contains(fileExtension.ToLower());
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rnd = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static ExerciseType ToExerciseType(this string inputString)
        {
            string exerciseType = inputString.Split('-')[1];

            Enum.TryParse(exerciseType, out ExerciseType enumValue);
            return enumValue;
        }

        public static SpeciesType ToSpeciesType(this string inputString)
        {
            string speciesType = inputString.Split('-')[0];

            Enum.TryParse(speciesType, out SpeciesType enumValue);
            return enumValue;
        }
    }
}
