using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAllPaged
{
    public class RequisitionStageConfigGetAllPagedHandler : IRequestHandler<RequisitionStageConfigGetAllPagedQuery, PagedResult<RequisitionStageConfigResponse>>
    {
        private readonly IRequisitionStageConfigService _requisitionStageConfigService;

        public RequisitionStageConfigGetAllPagedHandler(IRequisitionStageConfigService requisitionStageConfigService)
        {
            _requisitionStageConfigService = requisitionStageConfigService;
        }

        public async Task<PagedResult<RequisitionStageConfigResponse>> Handle(RequisitionStageConfigGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _requisitionStageConfigService.GetPaginatedAsync(query.Request);
        }
    }
}
