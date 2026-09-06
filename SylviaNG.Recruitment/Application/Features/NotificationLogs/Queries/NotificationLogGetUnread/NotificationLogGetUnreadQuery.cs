using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetUnread
{
    public class NotificationLogGetUnreadQuery : IRequest<List<NotificationLogResponse>>
    {
    }
}
