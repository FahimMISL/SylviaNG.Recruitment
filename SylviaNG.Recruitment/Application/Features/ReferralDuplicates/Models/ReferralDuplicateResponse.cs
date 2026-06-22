using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models
{
    public class ReferralDuplicateResponse
    {
        public long ReferralDuplicateId { get; set; }
        public long PrimaryReferralId { get; set; }
        public long DuplicateReferralId { get; set; }
        public string MatchField { get; set; } = string.Empty;
        public DuplicateResolutionEnum Resolution { get; set; }
        public long? ResolvedByUserId { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
