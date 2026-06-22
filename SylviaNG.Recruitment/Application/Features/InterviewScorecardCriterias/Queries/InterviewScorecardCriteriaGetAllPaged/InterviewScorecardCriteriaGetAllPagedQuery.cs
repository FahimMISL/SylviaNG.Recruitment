using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAllPaged
{
    public class InterviewScorecardCriteriaGetAllPagedQuery : IRequest<PagedResult<InterviewScorecardCriteriaResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewScorecardCriteriaGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
