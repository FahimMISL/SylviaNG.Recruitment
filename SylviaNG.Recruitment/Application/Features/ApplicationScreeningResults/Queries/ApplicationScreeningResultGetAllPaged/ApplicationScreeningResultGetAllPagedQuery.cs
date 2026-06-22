using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAllPaged
{
    public class ApplicationScreeningResultGetAllPagedQuery : IRequest<PagedResult<ApplicationScreeningResultResponse>>
    {
        public PagedRequest Request { get; set; }

        public ApplicationScreeningResultGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
