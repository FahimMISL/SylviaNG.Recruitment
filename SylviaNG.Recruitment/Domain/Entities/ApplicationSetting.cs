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
}
