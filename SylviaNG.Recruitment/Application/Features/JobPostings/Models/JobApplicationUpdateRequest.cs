using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    public class JobApplicationUpdateRequest
    {
        public ApplicationStatusEnum? ApplicationStatus { get; set; }
        public string? CandidatePhone { get; set; }
        public string? ResumeUrl { get; set; }
        public string? CoverLetter { get; set; }
        public bool? IsActive { get; set; }
    }
}
