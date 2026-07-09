using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetById
{
    public class HiringPipelineGetByIdHandler : IRequestHandler<HiringPipelineGetByIdQuery, HiringPipelineResponse>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineGetByIdHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<HiringPipelineResponse> Handle(HiringPipelineGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _hiringPipelineService.GetByIdAsync(query.HiringPipelineId);
        }
    }
}
