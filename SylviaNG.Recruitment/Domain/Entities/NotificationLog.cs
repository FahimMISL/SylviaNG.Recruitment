using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09 Feature 2: one row per (event, recipient) dispatch attempt made by
/// NotificationDispatchService - Sent/Failed/Skipped outcome, never thrown. Built now (not
/// deferred to Feature 3) so US-078/079's notification log + in-app bell has a table to query
/// instead of retrofitting one.
/// </summary>
public class NotificationLog : Audit
{
    public long NotificationLogId { get; set; }
    public RecruitmentEventEnum RecruitmentEvent { get; set; }
    public NotificationChannelEnum Channel { get; set; }
    public NotificationRecipientTypeEnum RecipientType { get; set; }
    public string RecipientAddress { get; set; } = string.Empty;

    /// <summary>Null when Status=Skipped because no active EventTemplateMapping was found.</summary>
    public long? NotificationTemplateId { get; set; }

    /// <summary>Null for events with no JobApplication context (e.g. AccountCreatedOtp).</summary>
    public long? JobApplicationId { get; set; }

    public string? RenderedSubject { get; set; }
    public NotificationStatusEnum DeliveryStatus { get; set; }
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }

    public NotificationTemplate? NotificationTemplate { get; set; }
    public JobApplication? JobApplication { get; set; }
}
