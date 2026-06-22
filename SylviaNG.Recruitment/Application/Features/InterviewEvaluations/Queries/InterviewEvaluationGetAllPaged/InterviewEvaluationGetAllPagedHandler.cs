using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAllPaged
{
    public class InterviewEvaluationGetAllPagedHandler : IRequestHandler<InterviewEvaluationGetAllPagedQuery, PagedResult<InterviewEvaluationResponse>>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationGetAllPagedHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task<PagedResult<InterviewEvaluationResponse>> Handle(InterviewEvaluationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationService.GetPaginatedAsync(query.Request);
        }
    }
}
