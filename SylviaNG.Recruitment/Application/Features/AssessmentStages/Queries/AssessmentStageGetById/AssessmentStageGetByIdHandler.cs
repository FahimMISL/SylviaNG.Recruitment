using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetById
{
    public class AssessmentStageGetByIdHandler : IRequestHandler<AssessmentStageGetByIdQuery, AssessmentStageResponse>
    {
        private readonly IAssessmentStageService _service;

        public AssessmentStageGetByIdHandler(IAssessmentStageService service)
        {
            _service = service;
        }

        public async Task<AssessmentStageResponse> Handle(AssessmentStageGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.AssessmentStageId);
        }
    }
}
