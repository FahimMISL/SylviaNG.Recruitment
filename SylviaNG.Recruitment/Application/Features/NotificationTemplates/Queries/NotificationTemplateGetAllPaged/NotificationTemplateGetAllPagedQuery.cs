using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplateGetAllPaged
{
    public class NotificationTemplateGetAllPagedQuery : IRequest<PagedResult<NotificationTemplateResponse>>
    {
        public PagedRequest Request { get; set; }

        public NotificationTemplateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
