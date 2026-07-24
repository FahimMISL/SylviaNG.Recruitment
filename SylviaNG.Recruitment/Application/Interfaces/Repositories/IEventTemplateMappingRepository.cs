using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IEventTemplateMappingRepository : IRepository<EventTemplateMapping>
    {
        Task<bool> ExistsAsync(RecruitmentEventEnum recruitmentEvent, NotificationChannelEnum channel, NotificationRecipientTypeEnum recipientType, long? excludeId = null);
        Task<List<EventTemplateMapping>> GetAllWithTemplateAsync();
        Task<EventTemplateMapping?> GetByIdWithTemplateAsync(long eventTemplateMappingId);

        /// <summary>EP-09 Feature 2 runtime resolver: the one active mapping (if any) for this
        /// event/channel/recipient combination, with an active template. Null if unconfigured -
        /// callers treat that as Skipped, not an error.</summary>
        Task<EventTemplateMapping?> GetActiveMappingAsync(RecruitmentEventEnum recruitmentEvent, NotificationChannelEnum channel, NotificationRecipientTypeEnum recipientType);
    }
}
