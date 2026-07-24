using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    public class NotificationTemplateCreateRequest
    {
        public NotificationChannelEnum Channel { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
