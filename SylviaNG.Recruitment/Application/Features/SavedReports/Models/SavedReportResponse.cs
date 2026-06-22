namespace SylviaNG.Recruitment.Application.Features.SavedReports.Models
{
    public class SavedReportResponse
    {
        public long SavedReportId { get; set; }
        public long CreatedByUserId { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? FilterCriteriaJson { get; set; }
        public string? ColumnConfigJson { get; set; }
        public bool IsActive { get; set; }
    }
}
