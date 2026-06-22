using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Queries.InterviewEvaluationScoreGetById
{
    public class InterviewEvaluationScoreGetByIdHandler : IRequestHandler<InterviewEvaluationScoreGetByIdQuery, InterviewEvaluationScoreResponse>
    {
        private readonly IInterviewEvaluationScoreService _service;

        public InterviewEvaluationScoreGetByIdHandler(IInterviewEvaluationScoreService service)
        {
            _service = service;
        }

        public async Task<InterviewEvaluationScoreResponse> Handle(InterviewEvaluationScoreGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewEvaluationScoreId);
        }
    }
}
