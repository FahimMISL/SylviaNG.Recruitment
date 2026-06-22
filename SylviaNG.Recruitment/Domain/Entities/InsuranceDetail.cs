using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class InsuranceDetail : Audit
{
    public long InsuranceDetailId { get; set; }
    public long PreBoardingProfileId { get; set; }
    public string InsuranceType { get; set; } = string.Empty;
    public string? ProviderName { get; set; }
    public string? PolicyNumber { get; set; }
    public string? BeneficiaryName { get; set; }
    public string? BeneficiaryRelationship { get; set; }
    public string? DocumentFileUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public PreBoardingProfile PreBoardingProfile { get; set; } = null!;
}
