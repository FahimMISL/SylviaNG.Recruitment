using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetById
{
    public class RequisitionStageConfigGetByIdHandler : IRequestHandler<RequisitionStageConfigGetByIdQuery, RequisitionStageConfigResponse>
    {
        private readonly IRequisitionStageConfigService _service;

        public RequisitionStageConfigGetByIdHandler(IRequisitionStageConfigService service)
        {
            _service = service;
        }

        public async Task<RequisitionStageConfigResponse> Handle(RequisitionStageConfigGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RequisitionStageConfigId);
        }
    }
}
