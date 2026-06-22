using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class OfferCompensation : Audit
{
    public long OfferCompensationId { get; set; }
    public long JobApplicationId { get; set; }
    public long? FitmentDataId { get; set; }
    public string ComponentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public string? Frequency { get; set; }
    public bool IsWithinPermittedRange { get; set; } = true;
    public bool RequiresAdditionalApproval { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
    public FitmentData? FitmentData { get; set; }
}
