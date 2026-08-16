using System.Text.Json.Serialization;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Features.Exams.Models
{
    public class ExamResponse
    {
        public long ExamId { get; set; }
        public long JobPostingId { get; set; }
        public string Title { get; set; } = string.Empty;
        [JsonConverter(typeof(LocalDateTimeJsonConverter))]
        public DateTime ScheduledStartAt { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMarks { get; set; }
        public ExamTypeEnum ExamType { get; set; }
        public long? ExamVenueId { get; set; }
        public string? ExamVenueName { get; set; }
        public List<long> QuestionGroupIds { get; set; } = new();
        public List<string> QuestionGroupNames { get; set; } = new();
        public DateTime? SeatPlanGeneratedAt { get; set; }
        public bool IsActive { get; set; }
        public bool ShowResultsToCandidate { get; set; }
    }
}
