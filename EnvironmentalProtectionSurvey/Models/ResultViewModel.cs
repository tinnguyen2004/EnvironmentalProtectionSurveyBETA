// Create a new class file, for example, ResultViewModel.cs
namespace EnvironmentalProtectionSurvey.Models
{
    public class ResultViewModel
    {
        public string QuestionText { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
