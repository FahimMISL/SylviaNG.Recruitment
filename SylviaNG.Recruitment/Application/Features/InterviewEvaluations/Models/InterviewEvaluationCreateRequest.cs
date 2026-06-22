using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models
{
    public class InterviewEvaluationCreateRequest
    {
        public long InterviewId { get; set; }
        public long PanelistUserId { get; set; }
        public decimal? OverallScore { get; set; }
        public InterviewResultEnum Recommendation { get; set; }
        public string? Commentary { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
