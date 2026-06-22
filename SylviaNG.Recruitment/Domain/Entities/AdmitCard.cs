using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class AdmitCard : Audit
{
    public long AdmitCardId { get; set; }
    public long ExamCandidateId { get; set; }
    public long? ExamSeatPlanId { get; set; }
    public string? FileUrl { get; set; }
    public NotificationChannelEnum DeliveryChannel { get; set; }
    public DeliveryStatusEnum DeliveryStatus { get; set; } = DeliveryStatusEnum.Pending;
    public DateTime? DeliveredAt { get; set; }
    public string? FailureReason { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ExamCandidate ExamCandidate { get; set; } = null!;
    public ExamSeatPlan? ExamSeatPlan { get; set; }
}
