namespace SylviaNG.Recruitment.Application.Features.SavedReports.Models
{
    public class SavedReportCreateRequest
    {
        public long CreatedByUserId { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string? FilterCriteriaJson { get; set; }
        public string? ColumnConfigJson { get; set; }
    }
}
