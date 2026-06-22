using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAllPaged
{
    public class InterviewEvaluationGetAllPagedQuery : IRequest<PagedResult<InterviewEvaluationResponse>>
    {
        public PagedRequest Request { get; set; }

        public InterviewEvaluationGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
