using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Models
{
    public class VerificationItemResponse
    {
        public long VerificationItemId { get; set; }
        public long VerificationWorkflowId { get; set; }
        public VerificationTypeEnum VerificationType { get; set; }
        public VerificationStatusEnum ItemStatus { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public string? EvidenceFileUrl { get; set; }
        public long? VerifiedByUserId { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
