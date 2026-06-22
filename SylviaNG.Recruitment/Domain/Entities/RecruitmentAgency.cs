using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class RecruitmentAgency : Audit
{
    public long RecruitmentAgencyId { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AgreementDetails { get; set; }
    public DateTime? AgreementStartDate { get; set; }
    public DateTime? AgreementEndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Referral> Referrals { get; set; } = new List<Referral>();
}
