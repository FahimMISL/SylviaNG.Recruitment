using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetById
{
    public class InterviewEvaluationGetByIdHandler : IRequestHandler<InterviewEvaluationGetByIdQuery, InterviewEvaluationResponse>
    {
        private readonly IInterviewEvaluationService _service;

        public InterviewEvaluationGetByIdHandler(IInterviewEvaluationService service)
        {
            _service = service;
        }

        public async Task<InterviewEvaluationResponse> Handle(InterviewEvaluationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewEvaluationId);
        }
    }
}
