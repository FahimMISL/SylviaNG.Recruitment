namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models
{
    // Channel and Code are immutable after create - changing either would silently break any
    // EventTemplateMapping already pointing at this template's channel/identity.
    public class NotificationTemplateUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
