using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Exams.Models
{
    public class ExamCreateRequest
    {
        public long RequisitionId { get; set; }
        public long? AssessmentStageId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public ExamStatusEnum ExamStatus { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal? PassMarks { get; set; }
        public string? Instructions { get; set; }
    }
}
