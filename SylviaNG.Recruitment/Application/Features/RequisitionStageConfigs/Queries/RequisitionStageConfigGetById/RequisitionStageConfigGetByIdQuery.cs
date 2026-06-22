using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetById
{
    public class RequisitionStageConfigGetByIdQuery : IRequest<RequisitionStageConfigResponse>
    {
        public long RequisitionStageConfigId { get; set; }

        public RequisitionStageConfigGetByIdQuery(long requisitionStageConfigId)
        {
            RequisitionStageConfigId = requisitionStageConfigId;
        }
    }
}
