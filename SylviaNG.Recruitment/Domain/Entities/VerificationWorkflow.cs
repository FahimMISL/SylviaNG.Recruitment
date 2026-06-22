using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class VerificationWorkflow : Audit
{
    public long VerificationWorkflowId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public VerificationStatusEnum OverallStatus { get; set; } = VerificationStatusEnum.Pending;
    public long InitiatedByUserId { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
    public User InitiatedByUser { get; set; } = null!;
    public ICollection<VerificationItem> Items { get; set; } = new List<VerificationItem>();
    public MedicalTest? MedicalTest { get; set; }
}
