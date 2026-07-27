using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Models
{
    /// <summary>The candidate's exam paper, returned once they start (US-058 AC2). Options never
    /// carry IsCorrect - the answer key must not reach the candidate.</summary>
    public class ExamPaperResponse
    {
        public long ExamEnrollmentId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime DeadlineAt { get; set; }
        public List<ExamPaperQuestionResponse> Questions { get; set; } = new();
    }

    public class ExamPaperQuestionResponse
    {
        public long ExamQuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionTypeEnum QuestionType { get; set; }
        public decimal Marks { get; set; }
        public List<ExamPaperOptionResponse> Options { get; set; } = new();
    }

    public class ExamPaperOptionResponse
    {
        public long ExamQuestionOptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }
}
