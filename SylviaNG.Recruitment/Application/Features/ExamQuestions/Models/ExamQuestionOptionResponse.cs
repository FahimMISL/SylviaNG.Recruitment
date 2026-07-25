namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Models
{
    public class ExamQuestionOptionResponse
    {
        public long ExamQuestionOptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int DisplayOrder { get; set; }
    }
}
