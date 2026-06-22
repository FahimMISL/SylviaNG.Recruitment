using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Models
{
    public class ReferralUpdateRequest
    {
        public long? CandidateId { get; set; }
        public long? JobPostingId { get; set; }
        public ReferralSourceEnum? Source { get; set; }
        public long? ReferrerEmployeeId { get; set; }
        public long? RecruitmentAgencyId { get; set; }
        public string? ReferrerName { get; set; }
        public string? ReferrerContact { get; set; }
        public ReferralStatusEnum? ReferralStatus { get; set; }
        public bool? IsActive { get; set; }
    }
}
