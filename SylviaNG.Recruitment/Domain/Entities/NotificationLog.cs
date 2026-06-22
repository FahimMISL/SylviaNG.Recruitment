using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class NotificationLog : Audit
{
    public long NotificationLogId { get; set; }
    public long? NotificationEventId { get; set; }
    public long? CandidateId { get; set; }
    public long? JobApplicationId { get; set; }
    public NotificationChannelEnum Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public DeliveryStatusEnum DeliveryStatus { get; set; } = DeliveryStatusEnum.Pending;
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public NotificationEvent? NotificationEvent { get; set; }
    public Candidate? Candidate { get; set; }
    public JobApplication? JobApplication { get; set; }
}
