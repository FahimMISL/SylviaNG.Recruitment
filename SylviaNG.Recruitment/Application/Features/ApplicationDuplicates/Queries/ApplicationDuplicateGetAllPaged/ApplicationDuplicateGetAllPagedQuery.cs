using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAllPaged
{
    public class ApplicationDuplicateGetAllPagedQuery : IRequest<PagedResult<ApplicationDuplicateResponse>>
    {
        public PagedRequest Request { get; set; }

        public ApplicationDuplicateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
