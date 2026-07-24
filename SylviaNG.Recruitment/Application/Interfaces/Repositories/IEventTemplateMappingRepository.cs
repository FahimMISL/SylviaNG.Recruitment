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
    }
}
