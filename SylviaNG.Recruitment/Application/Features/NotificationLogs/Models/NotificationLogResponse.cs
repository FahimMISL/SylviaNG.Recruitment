using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Models
{
    public class NotificationLogResponse
    {
        public long NotificationLogId { get; set; }
        public RecruitmentEventEnum RecruitmentEvent { get; set; }
        public NotificationChannelEnum Channel { get; set; }
        public NotificationRecipientTypeEnum RecipientType { get; set; }
        public string RecipientAddress { get; set; } = string.Empty;

        /// <summary>Candidate's full name for Candidate rows; "Admin / HR" for AdminHr rows.</summary>
        public string RecipientName { get; set; } = string.Empty;

        public string? RenderedSubject { get; set; }
        public NotificationStatusEnum DeliveryStatus { get; set; }
        public DateTime? SentAt { get; set; }
        public string? FailureReason { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public long? JobApplicationId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
