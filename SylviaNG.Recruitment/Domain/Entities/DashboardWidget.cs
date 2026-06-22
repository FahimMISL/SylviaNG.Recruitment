using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class DashboardWidget : Audit
{
    public long DashboardWidgetId { get; set; }
    public long UserId { get; set; }
    public string WidgetType { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ConfigJson { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User User { get; set; } = null!;
}
