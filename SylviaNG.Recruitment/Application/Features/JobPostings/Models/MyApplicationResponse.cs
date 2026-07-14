using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Candidate's own view of one submitted application (US-040 AC1/AC2/AC3).</summary>
    public class MyApplicationResponse
    {
        public long JobApplicationId { get; set; }
        public long JobPostingId { get; set; }
        public string? JobPostingTitle { get; set; }
        public DateTime? AppliedDate { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public bool CanWithdraw { get; set; }
        public List<MyApplicationInterviewResponse> Interviews { get; set; } = new();
    }

    /// <summary>Interview fields safe to show a candidate - no interviewer feedback/result (US-040 AC3).</summary>
    public class MyApplicationInterviewResponse
    {
        public long InterviewId { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? Location { get; set; }
        public string? MeetingLink { get; set; }
        public string? Round { get; set; }
    }
}
