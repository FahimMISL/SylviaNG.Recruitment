namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Models
{
    public class ExamAnswerResponse
    {
        public long ExamAnswerId { get; set; }
        public long ExamCandidateId { get; set; }
        public long QuestionId { get; set; }
        public long? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
        public decimal? MarksAwarded { get; set; }
        public bool? IsCorrect { get; set; }
        public bool IsActive { get; set; }
    }
}
