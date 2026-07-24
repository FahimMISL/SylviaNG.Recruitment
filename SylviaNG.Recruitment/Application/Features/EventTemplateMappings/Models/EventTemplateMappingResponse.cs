using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models
{
    public class EventTemplateMappingResponse
    {
        public long EventTemplateMappingId { get; set; }
        public RecruitmentEventEnum RecruitmentEvent { get; set; }
        public NotificationChannelEnum Channel { get; set; }
        public NotificationRecipientTypeEnum RecipientType { get; set; }
        public long NotificationTemplateId { get; set; }
        public string NotificationTemplateName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
