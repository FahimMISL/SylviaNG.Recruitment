using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class JobApplicationCreateRequest
    {
        public long JobPostingId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateEmail { get; set; }
        public string? CandidatePhone { get; set; }
        public string? CandidateNationalId { get; set; }
        public string? ResumeUrl { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationSourceEnum Source { get; set; } = ApplicationSourceEnum.Admin;
    }
}
