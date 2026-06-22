using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAll
{
    public class RequisitionStageConfigGetAllHandler : IRequestHandler<RequisitionStageConfigGetAllQuery, List<RequisitionStageConfigResponse>>
    {
        private readonly IRequisitionStageConfigService _service;

        public RequisitionStageConfigGetAllHandler(IRequisitionStageConfigService service)
        {
            _service = service;
        }

        public async Task<List<RequisitionStageConfigResponse>> Handle(RequisitionStageConfigGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
