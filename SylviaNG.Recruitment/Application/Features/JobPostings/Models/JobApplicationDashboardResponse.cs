using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>ATS dashboard row (US-035 AC1).</summary>
    public class JobApplicationDashboardResponse
    {
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public long JobPostingId { get; set; }
        public string? JobPostingTitle { get; set; }
        public ApplicationSourceEnum Source { get; set; }
        public DateTime? AppliedDate { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
    }
}
