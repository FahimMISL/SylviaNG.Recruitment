using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.CandidateNotificationLogGetUnread
{
    public class CandidateNotificationLogGetUnreadQuery : IRequest<List<NotificationLogResponse>>
    {
    }
}
