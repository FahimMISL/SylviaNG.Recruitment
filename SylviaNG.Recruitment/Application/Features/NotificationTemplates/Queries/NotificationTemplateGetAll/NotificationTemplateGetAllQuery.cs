using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAll
{
    public class NotificationTemplateGetAllQuery : IRequest<List<NotificationTemplateResponse>>
    {
    }
}
