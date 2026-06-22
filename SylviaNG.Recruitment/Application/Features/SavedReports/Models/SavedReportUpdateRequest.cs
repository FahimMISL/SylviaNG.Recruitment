namespace SylviaNG.Recruitment.Application.Features.SavedReports.Models
{
    public class SavedReportUpdateRequest
    {
        public long? CreatedByUserId { get; set; }
        public string? ReportName { get; set; }
        public string? Description { get; set; }
        public string? ReportType { get; set; }
        public string? FilterCriteriaJson { get; set; }
        public string? ColumnConfigJson { get; set; }
        public bool? IsActive { get; set; }
    }
}
