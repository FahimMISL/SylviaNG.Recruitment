namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Models
{
    public class QuestionOptionResponse
    {
        public long QuestionOptionId { get; set; }
        public long QuestionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
