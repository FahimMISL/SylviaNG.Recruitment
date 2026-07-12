using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>Bulk status move (US-035 AC5), e.g. move 50 selected applications to Screening at once.</summary>
    public class JobApplicationBulkStatusUpdateRequest
    {
        public List<long> JobApplicationIds { get; set; } = new();
        public ApplicationStatusEnum ToStatus { get; set; }
        public long? ReasonId { get; set; }
        public string? Note { get; set; }
    }
}
