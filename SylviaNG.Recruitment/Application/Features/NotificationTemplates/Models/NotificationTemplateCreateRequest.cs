using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    public class NotificationTemplateCreateRequest
    {
        public string TemplateName { get; set; } = string.Empty;
        public NotificationChannelEnum Channel { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? PlaceholdersJson { get; set; }
    }
}
