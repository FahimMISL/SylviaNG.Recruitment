using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class FitmentData : Audit
{
    public long FitmentDataId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public string? ProposedGrade { get; set; }
    public string? ProposedRole { get; set; }
    public string? Location { get; set; }
    public string? SalaryStructureJson { get; set; }
    public string? PayrollSource { get; set; }
    public bool IsFetchedFromPayroll { get; set; }
    public bool IsManualEntry { get; set; }
    public DateTime FetchedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
}
