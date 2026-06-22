using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll
{
    public class NotificationLogGetAllQuery : IRequest<List<NotificationLogResponse>>
    {
    }
}
