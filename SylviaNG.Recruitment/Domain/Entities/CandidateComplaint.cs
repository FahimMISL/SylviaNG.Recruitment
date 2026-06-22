using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class CandidateComplaint : Audit
{
    public long CandidateComplaintId { get; set; }
    public long CandidateId { get; set; }
    public long? JobApplicationId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatusEnum ComplaintStatus { get; set; } = ComplaintStatusEnum.Open;
    public long? AssignedToUserId { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public JobApplication? JobApplication { get; set; }
    public User? AssignedToUser { get; set; }
}
