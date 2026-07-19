using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Models
{
    public class ExamQuestionUpdateRequest
    {
        public long QuestionGroupId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionTypeEnum QuestionType { get; set; }
        public DifficultyLevelEnum DifficultyLevel { get; set; }
        public decimal Marks { get; set; }
        public string? Explanation { get; set; }
        public string? ModelAnswer { get; set; }
        public List<ExamQuestionOptionRequest> Options { get; set; } = new();
    }
}
