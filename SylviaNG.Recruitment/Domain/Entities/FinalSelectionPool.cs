using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class FinalSelectionPool : Audit
{
    public long FinalSelectionPoolId { get; set; }
    public long CandidateId { get; set; }
    public long JobApplicationId { get; set; }
    public DateTime? ExpectedJoiningDate { get; set; }
    public string? JoiningBatch { get; set; }
    public string? OnboardingChecklistJson { get; set; }
    public bool HasJoined { get; set; }
    public DateTime? ActualJoiningDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication JobApplication { get; set; } = null!;
}
