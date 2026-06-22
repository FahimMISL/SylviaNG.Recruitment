using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetByJobPostingId
{
    public class HiringPipelineStageGetByJobPostingIdHandler : IRequestHandler<HiringPipelineStageGetByJobPostingIdQuery, List<HiringPipelineStageResponse>>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageGetByJobPostingIdHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task<List<HiringPipelineStageResponse>> Handle(HiringPipelineStageGetByJobPostingIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByJobPostingIdAsync(query.JobPostingId);
        }
    }
}
