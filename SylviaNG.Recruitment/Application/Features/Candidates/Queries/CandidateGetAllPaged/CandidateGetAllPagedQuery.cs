using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAllPaged
{
    public class CandidateGetAllPagedQuery : IRequest<PagedResult<CandidateResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
