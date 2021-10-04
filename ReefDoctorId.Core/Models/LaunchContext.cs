namespace ReefDoctorId.Core.Models
{
    public class LaunchContext
    {
        public ExerciseType ExerciseType { get; set; }
        public SpeciesType SpeciesType { get; set; }
        public SurveyLevel SurveyLevel { get; set; }
        public int ExamNumber { get; set; }
    }
}