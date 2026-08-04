using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-13 US-104: one row per queued export request. The generated file is persisted via
/// IFileStorageService (ContentObjectKey points at the stored object/local path) rather than
/// inline in the DB - see ExportRequestWorker for the write path and ExportRequestService for
/// the read/download path.
/// </summary>
public class ExportRequest : Audit
{
    public long ExportRequestId { get; set; }
    public ExportTypeEnum ExportType { get; set; }
    public ExportFormatEnum Format { get; set; }

    /// <summary>Serialized JobApplicationAttributeFilterRequest used for this request - kept for
    /// audit/troubleshooting visibility, not re-parsed by anything.</summary>
    public string? FilterJson { get; set; }

    /// <summary>Serialized (JSON array of long) JobApplicationIds matched by the filter, resolved
    /// once at request time via the same GetDashboardMatchingIdsAsync path the ATS dashboard/bulk
    /// actions already use. The worker generates the file from this fixed snapshot rather than
    /// re-running the filter, so a request always exports exactly what matched when it was queued.</summary>
    public string JobApplicationIdsJson { get; set; } = "[]";

    /// <summary>Shadows Audit.Status (int) - same pattern OfferLetter.Status/PreBoardingSubmission.Status
    /// already use, since Audit's int Status is soft-delete/lifecycle bookkeeping unrelated to this enum.</summary>
    public new ExportRequestStatusEnum Status { get; set; } = ExportRequestStatusEnum.Pending;

    public string? RequestedByUserName { get; set; }
    public string? RequestedByEmail { get; set; }

    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>Retention cutoff (RequestedAt + retention window) - swept by ExportRequestWorker
    /// regardless of whether the file was ever downloaded.</summary>
    public DateTime ExpiresAt { get; set; }

    public string? ContentObjectKey { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public int? RowCount { get; set; }
    public string? FailureReason { get; set; }
}
