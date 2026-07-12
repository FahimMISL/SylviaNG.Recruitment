using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Moves an application to a new status (US-036). ReasonId is required when ToStatus is Rejected/Withdrawn.</summary>
    public class JobApplicationStatusUpdateRequest
    {
        public ApplicationStatusEnum ToStatus { get; set; }
        public long? ReasonId { get; set; }
        public string? Note { get; set; }
    }
}
