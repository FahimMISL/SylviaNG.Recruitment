using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAll
{
    public class AssessmentStageGetAllHandler : IRequestHandler<AssessmentStageGetAllQuery, List<AssessmentStageResponse>>
    {
        private readonly IAssessmentStageService _service;

        public AssessmentStageGetAllHandler(IAssessmentStageService service)
        {
            _service = service;
        }

        public async Task<List<AssessmentStageResponse>> Handle(AssessmentStageGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
