using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAll
{
    public class InterviewScorecardCriteriaGetAllHandler : IRequestHandler<InterviewScorecardCriteriaGetAllQuery, List<InterviewScorecardCriteriaResponse>>
    {
        private readonly IInterviewScorecardCriteriaService _service;

        public InterviewScorecardCriteriaGetAllHandler(IInterviewScorecardCriteriaService service)
        {
            _service = service;
        }

        public async Task<List<InterviewScorecardCriteriaResponse>> Handle(InterviewScorecardCriteriaGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
