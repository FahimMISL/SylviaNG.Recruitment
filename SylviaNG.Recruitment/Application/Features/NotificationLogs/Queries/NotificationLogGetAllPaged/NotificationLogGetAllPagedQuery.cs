using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationLogs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Queries.NotificationLogGetAllPaged
{
    public class NotificationLogGetAllPagedQuery : IRequest<PagedResult<NotificationLogResponse>>
    {
        public PagedRequest Request { get; set; }

        public NotificationLogGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
