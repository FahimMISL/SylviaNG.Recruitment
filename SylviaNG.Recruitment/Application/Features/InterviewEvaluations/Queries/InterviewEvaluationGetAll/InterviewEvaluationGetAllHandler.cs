using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationGetAll
{
    public class InterviewEvaluationGetAllHandler : IRequestHandler<InterviewEvaluationGetAllQuery, List<InterviewEvaluationResponse>>
    {
        private readonly IInterviewEvaluationService _service;

        public InterviewEvaluationGetAllHandler(IInterviewEvaluationService service)
        {
            _service = service;
        }

        public async Task<List<InterviewEvaluationResponse>> Handle(InterviewEvaluationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
