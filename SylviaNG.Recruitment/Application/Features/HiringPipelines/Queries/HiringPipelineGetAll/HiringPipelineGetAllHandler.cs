using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetAll
{
    public class HiringPipelineGetAllHandler : IRequestHandler<HiringPipelineGetAllQuery, List<HiringPipelineResponse>>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineGetAllHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<List<HiringPipelineResponse>> Handle(HiringPipelineGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _hiringPipelineService.GetAllAsync();
        }
    }
}
