using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAllPaged
{
    public class InterviewEvaluationScoreGetAllPagedHandler : IRequestHandler<InterviewEvaluationScoreGetAllPagedQuery, PagedResult<InterviewEvaluationScoreResponse>>
    {
        private readonly IInterviewEvaluationScoreService _interviewEvaluationScoreService;

        public InterviewEvaluationScoreGetAllPagedHandler(IInterviewEvaluationScoreService interviewEvaluationScoreService)
        {
            _interviewEvaluationScoreService = interviewEvaluationScoreService;
        }

        public async Task<PagedResult<InterviewEvaluationScoreResponse>> Handle(InterviewEvaluationScoreGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationScoreService.GetPaginatedAsync(query.Request);
        }
    }
}
