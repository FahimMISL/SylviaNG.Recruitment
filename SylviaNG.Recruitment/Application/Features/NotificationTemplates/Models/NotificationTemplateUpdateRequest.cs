using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    public class NotificationTemplateUpdateRequest
    {
        public string? TemplateName { get; set; }
        public NotificationChannelEnum? Channel { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? PlaceholdersJson { get; set; }
        public bool? IsActive { get; set; }
    }
}
