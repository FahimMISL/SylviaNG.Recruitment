using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A configurable reason an HR user can pick when rejecting or withdrawing an application (US-036 AC3).
/// </summary>
public class ApplicationStatusReason : Audit
{
    public long ApplicationStatusReasonId { get; set; }
    public string Label { get; set; } = string.Empty;
    public ApplicationStatusEnum AppliesToStatus { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
