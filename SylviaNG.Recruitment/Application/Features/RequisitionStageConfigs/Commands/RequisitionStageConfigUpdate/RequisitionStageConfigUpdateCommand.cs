using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigUpdate
{
    public class RequisitionStageConfigUpdateCommand : IRequest<Unit>
    {
        public long RequisitionStageConfigId { get; set; }
        public RequisitionStageConfigUpdateRequest Request { get; set; }

        public RequisitionStageConfigUpdateCommand(long requisitionStageConfigId, RequisitionStageConfigUpdateRequest request)
        {
            RequisitionStageConfigId = requisitionStageConfigId;
            Request = request;
        }
    }
}
