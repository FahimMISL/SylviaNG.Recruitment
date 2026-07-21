using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Models
{
    public class ExamQuestionResponse
    {
        public long ExamQuestionId { get; set; }
        public long QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public QuestionTypeEnum QuestionType { get; set; }
        public DifficultyLevelEnum DifficultyLevel { get; set; }
        public decimal Marks { get; set; }
        public string? Explanation { get; set; }
        public string? ModelAnswer { get; set; }
        public bool IsActive { get; set; }
        public List<ExamQuestionOptionResponse> Options { get; set; } = new();
    }
}
