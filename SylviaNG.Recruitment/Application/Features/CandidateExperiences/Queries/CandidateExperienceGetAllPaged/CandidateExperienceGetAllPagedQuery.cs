using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Queries.CandidateExperienceGetAllPaged
{
    public class CandidateExperienceGetAllPagedQuery : IRequest<PagedResult<CandidateExperienceResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateExperienceGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
