using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>One entry of an application's status audit trail (US-036 AC2). HR-only view - Note is private.</summary>
    public class ApplicationStatusHistoryResponse
    {
        public long ApplicationStatusHistoryId { get; set; }
        public ApplicationStatusEnum? FromStatus { get; set; }
        public ApplicationStatusEnum ToStatus { get; set; }
        public string? ChangedByUserName { get; set; }
        public DateTime ChangedAt { get; set; }
        public long? ReasonId { get; set; }
        public string? ReasonLabel { get; set; }
        public string? Note { get; set; }
    }
}
