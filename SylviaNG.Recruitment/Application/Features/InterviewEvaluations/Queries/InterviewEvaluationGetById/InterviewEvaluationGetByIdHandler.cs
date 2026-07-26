using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetById
{
    public class InterviewEvaluationGetByIdHandler : IRequestHandler<InterviewEvaluationGetByIdQuery, InterviewEvaluationResponse>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationGetByIdHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task<InterviewEvaluationResponse> Handle(InterviewEvaluationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationService.GetByIdAsync(query.InterviewEvaluationId);
        }
    }
}
