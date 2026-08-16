using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09 Feature 2: one row per (event, recipient) dispatch attempt made by
/// NotificationDispatchService - Sent/Failed/Skipped outcome, never thrown. Built now (not
/// deferred to Feature 3) so US-078/079's notification log + in-app bell has a table to query
/// instead of retrofitting one.
/// </summary>
public class NotificationLog : Audit, ICompanyScoped
{
    public long NotificationLogId { get; set; }

    // Multi-tenant: the Company this dispatch belongs to, stamped by NotificationDispatchService
    // from the triggering JobApplication/recipient's company. Drives the per-company bell feed.
    public long? CompanyId { get; set; }
    public RecruitmentEventEnum RecruitmentEvent { get; set; }
    public NotificationChannelEnum Channel { get; set; }
    public NotificationRecipientTypeEnum RecipientType { get; set; }
    public string RecipientAddress { get; set; } = string.Empty;

    /// <summary>Null when Status=Skipped because no active EventTemplateMapping was found.</summary>
    public long? NotificationTemplateId { get; set; }

    /// <summary>Null for events with no JobApplication context (e.g. AccountCreatedOtp).</summary>
    public long? JobApplicationId { get; set; }

    public string? RenderedSubject { get; set; }

    /// <summary>Set alongside RenderedSubject at render time so a Failed row can be Retried without
    /// re-rendering placeholders (which are never persisted). Skipped rows never reach render, so
    /// this stays null for them - Retry only operates on Failed rows.</summary>
    public string? RenderedBody { get; set; }
    public NotificationStatusEnum DeliveryStatus { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }

    /// <summary>EP-09 Feature 3 (US-079): read state for the in-app HR/Admin notification bell.</summary>
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    public NotificationTemplate? NotificationTemplate { get; set; }
    public JobApplication? JobApplication { get; set; }
}
