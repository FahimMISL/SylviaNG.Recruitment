using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetById
{
    public class InterviewScorecardCriteriaGetByIdHandler : IRequestHandler<InterviewScorecardCriteriaGetByIdQuery, InterviewScorecardCriteriaResponse>
    {
        private readonly IInterviewScorecardCriteriaService _service;

        public InterviewScorecardCriteriaGetByIdHandler(IInterviewScorecardCriteriaService service)
        {
            _service = service;
        }

        public async Task<InterviewScorecardCriteriaResponse> Handle(InterviewScorecardCriteriaGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InterviewScorecardCriteriaId);
        }
    }
}
