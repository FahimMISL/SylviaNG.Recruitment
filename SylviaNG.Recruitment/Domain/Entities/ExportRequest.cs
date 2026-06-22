using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ExportRequest : Audit
{
    public long ExportRequestId { get; set; }
    public long RequestedByUserId { get; set; }
    public string ExportType { get; set; } = string.Empty;
    public string? FilterCriteriaJson { get; set; }
    public ExportStatusEnum ExportStatus { get; set; } = ExportStatusEnum.Queued;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileFormat { get; set; }
    public long? FileSizeBytes { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User RequestedByUser { get; set; } = null!;
}
