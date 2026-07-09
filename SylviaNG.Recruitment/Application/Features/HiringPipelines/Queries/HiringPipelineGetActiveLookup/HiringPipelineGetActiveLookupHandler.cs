using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetActiveLookup
{
    public class HiringPipelineGetActiveLookupHandler : IRequestHandler<HiringPipelineGetActiveLookupQuery, List<HiringPipelineLookupResponse>>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineGetActiveLookupHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<List<HiringPipelineLookupResponse>> Handle(HiringPipelineGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _hiringPipelineService.GetActiveLookupAsync();
        }
    }
}
