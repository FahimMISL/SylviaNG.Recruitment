using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationEvents.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Queries.NotificationEventGetAllPaged
{
    public class NotificationEventGetAllPagedQuery : IRequest<PagedResult<NotificationEventResponse>>
    {
        public PagedRequest Request { get; set; }

        public NotificationEventGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
