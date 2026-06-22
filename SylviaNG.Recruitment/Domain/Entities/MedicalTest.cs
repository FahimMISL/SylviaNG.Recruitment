using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class MedicalTest : Audit
{
    public long MedicalTestId { get; set; }
    public long VerificationWorkflowId { get; set; }
    public MedicalFitnessEnum FitnessStatus { get; set; } = MedicalFitnessEnum.Pending;
    public string? MedicalCenter { get; set; }
    public DateTime? TestDate { get; set; }
    public string? ResultFileUrl { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public VerificationWorkflow VerificationWorkflow { get; set; } = null!;
}
