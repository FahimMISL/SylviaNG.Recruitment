using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Models
{
    public class NotificationLogFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public NotificationChannelEnum? Channel { get; set; }
        public RecruitmentEventEnum? RecruitmentEvent { get; set; }
        public NotificationStatusEnum? DeliveryStatus { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
