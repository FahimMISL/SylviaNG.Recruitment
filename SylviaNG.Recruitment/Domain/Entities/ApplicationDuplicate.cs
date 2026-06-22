using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ApplicationDuplicate : Audit
{
    public long ApplicationDuplicateId { get; set; }
    public long PrimaryApplicationId { get; set; }
    public long DuplicateApplicationId { get; set; }
    public string MatchField { get; set; } = string.Empty;
    public DuplicateResolutionEnum Resolution { get; set; } = DuplicateResolutionEnum.Unresolved;
    public long? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public JobApplication PrimaryApplication { get; set; } = null!;
    public JobApplication DuplicateApplication { get; set; } = null!;
    public User? ResolvedByUser { get; set; }
}
