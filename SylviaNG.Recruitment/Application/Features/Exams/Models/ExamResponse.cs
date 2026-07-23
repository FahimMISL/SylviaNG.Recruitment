using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Exams.Models
{
    public class ExamResponse
    {
        public long ExamId { get; set; }
        public long JobPostingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledStartAt { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMarks { get; set; }
        public ExamTypeEnum ExamType { get; set; }
        public long? ExamVenueId { get; set; }
        public string? ExamVenueName { get; set; }
        public long? QuestionGroupId { get; set; }
        public string? QuestionGroupName { get; set; }
        public DateTime? SeatPlanGeneratedAt { get; set; }
        public bool IsActive { get; set; }
        public bool ShowResultsToCandidate { get; set; }
    }
}
