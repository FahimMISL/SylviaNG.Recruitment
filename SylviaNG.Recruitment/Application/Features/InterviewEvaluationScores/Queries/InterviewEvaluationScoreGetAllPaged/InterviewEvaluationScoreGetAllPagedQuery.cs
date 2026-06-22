using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAllPaged
{
    public class InterviewEvaluationScoreGetAllPagedQuery : IRequest<PagedResult<InterviewEvaluationScoreResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewEvaluationScoreGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
