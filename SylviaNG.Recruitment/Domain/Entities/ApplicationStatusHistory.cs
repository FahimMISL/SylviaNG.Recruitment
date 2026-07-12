using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Append-only audit trail of every status change on a JobApplication (US-036 AC2).
/// </summary>
public class ApplicationStatusHistory : Audit
{
    public long ApplicationStatusHistoryId { get; set; }
    public long JobApplicationId { get; set; }
    public ApplicationStatusEnum? FromStatus { get; set; }
    public ApplicationStatusEnum ToStatus { get; set; }
    public string? ChangedByUserName { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public long? ReasonId { get; set; }

    /// <summary>HR-private remark - never surfaced on any candidate-facing endpoint.</summary>
    public string? Note { get; set; }

    // Navigation properties
    public JobApplication JobApplication { get; set; } = null!;
    public ApplicationStatusReason? Reason { get; set; }
}
