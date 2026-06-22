using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAllPaged
{
    public class CandidateEducationGetAllPagedQuery : IRequest<PagedResult<CandidateEducationResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateEducationGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
