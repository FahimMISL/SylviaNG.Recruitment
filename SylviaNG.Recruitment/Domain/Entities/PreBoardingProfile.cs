using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class PreBoardingProfile : Audit
{
    public long PreBoardingProfileId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public PreBoardingStatusEnum ProfileStatus { get; set; } = PreBoardingStatusEnum.Pending;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public long? ValidatedByUserId { get; set; }
    public string? CorrectionComments { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public User? ValidatedByUser { get; set; }
    public ICollection<Nominee> Nominees { get; set; } = new List<Nominee>();
    public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    public ICollection<InsuranceDetail> InsuranceDetails { get; set; } = new List<InsuranceDetail>();
}
