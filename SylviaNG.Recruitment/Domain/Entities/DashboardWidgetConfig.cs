using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-14 US-105 AC5 (minimal build, ahead of full EP-15 access control): per-role visibility
/// toggle for a dashboard metric card. Seeded once with the 5 known widget keys, all visible by
/// default; Admin can hide a card per role without a code change.
/// </summary>
public class DashboardWidgetConfig : Audit
{
    public long DashboardWidgetConfigId { get; set; }
    public string WidgetKey { get; set; } = string.Empty;
    public bool IsVisibleForAdmin { get; set; } = true;
    public bool IsVisibleForHR { get; set; } = true;
}
