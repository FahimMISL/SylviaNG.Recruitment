using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAllPaged
{
    public class TalentPoolCandidateGetAllPagedQuery : IRequest<PagedResult<TalentPoolCandidateResponse>>
    {
        public PagedRequest Request { get; set; }

        public TalentPoolCandidateGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
