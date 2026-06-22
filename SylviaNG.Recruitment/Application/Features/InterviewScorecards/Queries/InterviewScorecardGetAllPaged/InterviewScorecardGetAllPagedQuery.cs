using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAllPaged
{
    public class InterviewScorecardGetAllPagedQuery : IRequest<PagedResult<InterviewScorecardResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewScorecardGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
