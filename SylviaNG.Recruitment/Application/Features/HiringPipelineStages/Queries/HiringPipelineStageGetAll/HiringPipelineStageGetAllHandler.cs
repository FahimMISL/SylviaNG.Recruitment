using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAll
{
    public class HiringPipelineStageGetAllHandler : IRequestHandler<HiringPipelineStageGetAllQuery, List<HiringPipelineStageResponse>>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageGetAllHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task<List<HiringPipelineStageResponse>> Handle(HiringPipelineStageGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
