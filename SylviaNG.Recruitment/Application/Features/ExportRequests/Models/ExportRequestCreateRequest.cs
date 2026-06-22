using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Models
{
    public class ExportRequestCreateRequest
    {
        public long RequestedByUserId { get; set; }
        public string ExportType { get; set; } = string.Empty;
        public string? FilterCriteriaJson { get; set; }
        public ExportStatusEnum ExportStatus { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileFormat { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
