using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class Nominee : Audit
{
    public long NomineeId { get; set; }
    public long PreBoardingProfileId { get; set; }
    public string NomineeName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? NationalIdNumber { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal? SharePercentage { get; set; }
    public string? IdProofFileUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public PreBoardingProfile PreBoardingProfile { get; set; } = null!;
}
