namespace EnvironmentalProtectionSurvey.Models
{
    public class FilledSurveyDetails
    {
        public int QuestionId { get; set; }
        public FilledSurvey FilledSurvey { get; set; }
        public List<Option> SelectedOptions { get; set; } // Thêm danh sách các Options đã chọn
        public Question Question { get; set; }
        public Survey Survey { get; set; }
    }
}
