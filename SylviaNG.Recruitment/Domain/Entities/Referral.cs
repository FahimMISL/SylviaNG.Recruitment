using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Referral : Audit
{
    public long ReferralId { get; set; }
    public long CandidateId { get; set; }
    public long? JobPostingId { get; set; }
    public ReferralSourceEnum Source { get; set; }
    public long? ReferrerEmployeeId { get; set; }
    public long? RecruitmentAgencyId { get; set; }
    public string? ReferrerName { get; set; }
    public string? ReferrerContact { get; set; }
    public ReferralStatusEnum ReferralStatus { get; set; } = ReferralStatusEnum.Submitted;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobPosting? JobPosting { get; set; }
    public Employee? ReferrerEmployee { get; set; }
    public RecruitmentAgency? RecruitmentAgency { get; set; }
}
