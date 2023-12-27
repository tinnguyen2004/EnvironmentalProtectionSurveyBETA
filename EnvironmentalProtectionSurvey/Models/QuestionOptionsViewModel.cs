namespace EnvironmentalProtectionSurvey.Models
{
    public class QuestionOptionsViewModel
    {
        public List<Question> Questions { get; set; }
        public List<Option> Options { get; set; }

        public QuestionOptionsViewModel(List<Question> questions, List<Option> options)
        {
            Questions = questions;
            Options = options;
        }
    }
}
