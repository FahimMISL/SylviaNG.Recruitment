using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class JobApplicationResponse
    {
        public long JobApplicationId { get; set; }
        public long JobPostingId { get; set; }
        public string? JobPostingTitle { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public string? ResumeUrl { get; set; }
        public ApplicationStatusEnum ApplicationStatus { get; set; }
        public DateTime? AppliedDate { get; set; }
        public bool IsActive { get; set; }
        public ApplicationSourceEnum Source { get; set; }

        // EP-17: set by JobApplicationService.SubmitAsync when the vacancy has an application
        // fee configured. PaymentRedirectUrl is null if the gateway couldn't be reached at
        // submit time even though payment is required - frontend should offer a retry.
        public bool PaymentRequired { get; set; }
        public string? PaymentRedirectUrl { get; set; }
    }
}
