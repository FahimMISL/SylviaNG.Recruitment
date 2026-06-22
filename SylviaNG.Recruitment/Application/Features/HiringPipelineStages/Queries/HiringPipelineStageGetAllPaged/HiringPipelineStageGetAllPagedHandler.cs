using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAllPaged
{
    public class HiringPipelineStageGetAllPagedHandler : IRequestHandler<HiringPipelineStageGetAllPagedQuery, PagedResult<HiringPipelineStageResponse>>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageGetAllPagedHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task<PagedResult<HiringPipelineStageResponse>> Handle(HiringPipelineStageGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetPaginatedAsync(query.Request);
        }
    }
}
