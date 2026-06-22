using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class EmergencyContact : Audit
{
    public long EmergencyContactId { get; set; }
    public long PreBoardingProfileId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public PreBoardingProfile PreBoardingProfile { get; set; } = null!;
}
