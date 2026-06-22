using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : Repository<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
