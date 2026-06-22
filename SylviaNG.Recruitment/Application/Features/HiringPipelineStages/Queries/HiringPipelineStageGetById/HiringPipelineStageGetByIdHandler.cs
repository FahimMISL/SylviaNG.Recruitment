using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetById
{
    public class HiringPipelineStageGetByIdHandler : IRequestHandler<HiringPipelineStageGetByIdQuery, HiringPipelineStageResponse>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageGetByIdHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task<HiringPipelineStageResponse> Handle(HiringPipelineStageGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.HiringPipelineStageId);
        }
    }
}
