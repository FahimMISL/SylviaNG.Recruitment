using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models
{
    public class EventTemplateMappingCreateRequest
    {
        public RecruitmentEventEnum RecruitmentEvent { get; set; }
        public NotificationChannelEnum Channel { get; set; }
        public NotificationRecipientTypeEnum RecipientType { get; set; }
        public long NotificationTemplateId { get; set; }
    }
}
