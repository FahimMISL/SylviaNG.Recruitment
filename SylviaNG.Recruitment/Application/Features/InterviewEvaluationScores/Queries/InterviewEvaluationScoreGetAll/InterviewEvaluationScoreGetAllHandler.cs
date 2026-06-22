using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetAll
{
    public class InterviewEvaluationScoreGetAllHandler : IRequestHandler<InterviewEvaluationScoreGetAllQuery, List<InterviewEvaluationScoreResponse>>
    {
        private readonly IInterviewEvaluationScoreService _service;

        public InterviewEvaluationScoreGetAllHandler(IInterviewEvaluationScoreService service)
        {
            _service = service;
        }

        public async Task<List<InterviewEvaluationScoreResponse>> Handle(InterviewEvaluationScoreGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
