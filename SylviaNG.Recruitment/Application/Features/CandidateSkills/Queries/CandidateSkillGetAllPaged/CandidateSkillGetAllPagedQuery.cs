using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateSkills.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Queries.CandidateSkillGetAllPaged
{
    public class CandidateSkillGetAllPagedQuery : IRequest<PagedResult<CandidateSkillResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateSkillGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
