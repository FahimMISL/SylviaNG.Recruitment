namespace SylviaNG.Recruitment.Application.Features.Analytics.Models
{
    /// <summary>EP-14 US-110 AC3: JobPostingId/DepartmentId/DateRange filter scope; DateFrom/DateTo
    /// filter on InterviewEvaluation.SubmittedAt.</summary>
    public class InterviewAnalyticsRequest
    {
        public long? JobPostingId { get; set; }
        public long? DepartmentId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
