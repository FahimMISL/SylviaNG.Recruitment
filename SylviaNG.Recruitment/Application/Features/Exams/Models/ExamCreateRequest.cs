using System.Text.Json.Serialization;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Features.Exams.Models
{
    public class ExamCreateRequest
    {
        public long JobPostingId { get; set; }
        public string Title { get; set; } = string.Empty;
        [JsonConverter(typeof(LocalDateTimeJsonConverter))]
        public DateTime ScheduledStartAt { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMarks { get; set; }
        public ExamTypeEnum ExamType { get; set; }
        public long? ExamVenueId { get; set; }
        public long? QuestionGroupId { get; set; }

        /// <summary>US-058 AC6: whether a candidate sees their score/pass-fail immediately after
        /// submitting an online exam, vs. just a submission confirmation.</summary>
        public bool ShowResultsToCandidate { get; set; }
    }
}
