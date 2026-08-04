using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetPaged
{
    public class ExportRequestGetPagedQuery : IRequest<PagedResult<ExportRequestResponse>>
    {
        public ExportRequestFilterRequest Filter { get; set; }

        public ExportRequestGetPagedQuery(ExportRequestFilterRequest filter)
        {
            Filter = filter;
        }
    }
}
