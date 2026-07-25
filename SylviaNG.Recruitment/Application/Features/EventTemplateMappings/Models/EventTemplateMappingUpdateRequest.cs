namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models
{
    // Event/Channel/RecipientType form the mapping's unique key - only which template it points at
    // and whether it's active can change. Repointing the key would just be delete+recreate.
    public class EventTemplateMappingUpdateRequest
    {
        public long NotificationTemplateId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
