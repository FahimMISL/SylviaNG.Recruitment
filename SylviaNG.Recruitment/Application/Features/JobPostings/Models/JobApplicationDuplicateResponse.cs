using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>One application within a detected duplicate group (US-038 AC2 side-by-side comparison).</summary>
    public class JobApplicationDuplicateItemResponse
    {
        public long JobApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public string? CandidateNationalId { get; set; }
        public ApplicationSourceEnum Source { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string? ResumeUrl { get; set; }
    }

    /// <summary>A group of applications to the same vacancy sharing email, NID, or phone (US-038 AC1).</summary>
    public class JobApplicationDuplicateGroupResponse
    {
        public List<JobApplicationDuplicateItemResponse> Applications { get; set; } = new();
        public List<string> MatchedOn { get; set; } = new();
    }
}
