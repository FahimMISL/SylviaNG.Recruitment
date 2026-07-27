using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAll
{
    public class NotificationLogGetAllQuery : IRequest<PagedResult<NotificationLogResponse>>
    {
        public NotificationLogFilterRequest Filter { get; }

        public NotificationLogGetAllQuery(NotificationLogFilterRequest filter)
        {
            Filter = filter;
        }
    }
}
