using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Models
{
    public class AdmitCardResponse
    {
        public long AdmitCardId { get; set; }
        public long ExamCandidateId { get; set; }
        public long? ExamSeatPlanId { get; set; }
        public string? FileUrl { get; set; }
        public NotificationChannelEnum DeliveryChannel { get; set; }
        public DeliveryStatusEnum DeliveryStatus { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? FailureReason { get; set; }
        public bool IsActive { get; set; }
    }
}
