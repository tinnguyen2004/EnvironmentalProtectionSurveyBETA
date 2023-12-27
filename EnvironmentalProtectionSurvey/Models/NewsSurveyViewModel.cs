namespace EnvironmentalProtectionSurvey.Models
{
    // ViewModel that combines News and Survey
    public class NewsSurveyViewModel
    {
        public IEnumerable<News> NewsList { get; set; }
        public IEnumerable<Survey> SurveyList { get; set; }
    }

}
