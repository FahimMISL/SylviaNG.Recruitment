using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class SavedReport : Audit
{
    public long SavedReportId { get; set; }
    public long CreatedByUserId { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string? FilterCriteriaJson { get; set; }
    public string? ColumnConfigJson { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
}
