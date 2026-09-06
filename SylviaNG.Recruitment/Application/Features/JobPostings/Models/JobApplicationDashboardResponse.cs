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

        // EP-14 US-109: tracker columns, populated from the application's current
        // JobApplicationStageProgress row (InProgress, falling back to the highest-DisplayOrder
        // Completed row) - null/false when there's no such row yet (brand-new application).
        public string? CurrentStageName { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public int? DaysInCurrentStage { get; set; }
        public bool IsStale { get; set; }
        public string? AssignedHrUserName { get; set; }
    }
}
