using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>
    /// Full application detail (US-035 AC4): the candidate snapshot captured at apply time
    /// (this application has no FK to CandidateProfile, so there is nothing else to join),
    /// the CV link, and the complete status-history audit trail.
    /// </summary>
    public class JobApplicationDetailResponse
    {
        public long JobApplicationId { get; set; }
        public long JobPostingId { get; set; }
        public string? JobPostingTitle { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public string? ResumeUrl { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public DateTime? AppliedDate { get; set; }
        public ApplicationSourceEnum Source { get; set; }
        public List<ApplicationStatusHistoryResponse> StatusHistory { get; set; } = new();
    }
}
