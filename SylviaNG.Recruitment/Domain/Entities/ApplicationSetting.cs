using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Single-row, tenant-wide recruitment settings (US-007 AC4). Seeded once with
/// ApplicationSettingId = 1; there is exactly one row, updated in place by Admin.
/// </summary>
public class ApplicationSetting : Audit
{
    public long ApplicationSettingId { get; set; }

    /// <summary>
    /// Minimum CandidateProfile completeness percentage (0-100) required before a candidate
    /// with an existing profile can submit a job application. 0 = gate disabled.
    /// </summary>
    public int MinimumProfileCompletenessPercentage { get; set; }

    /// <summary>
    /// EP-09 Feature 2: mailbox that receives the AdminHr leg of every dispatched notification
    /// (application submitted/withdrawn/status-changed, etc). Null = AdminHr notifications are
    /// skipped until an Admin configures this - there's no other HR contact address anywhere
    /// in the system today.
    /// </summary>
    public string? HrNotificationEmail { get; set; }

    /// <summary>
    /// EP-14 US-109 AC2: fallback "days in current stage" threshold used to highlight stale
    /// tracker rows when the application's current PipelineStage has no SlaDays configured.
    /// Null = no fallback threshold (only stage-specific SlaDays highlight, if any).
    /// </summary>
    public int? DefaultStaleDaysThreshold { get; set; }
}
