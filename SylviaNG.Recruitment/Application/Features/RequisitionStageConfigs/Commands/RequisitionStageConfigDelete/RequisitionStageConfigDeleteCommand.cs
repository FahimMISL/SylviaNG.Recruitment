using MediatR;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigDelete
{
    public class RequisitionStageConfigDeleteCommand : IRequest<Unit>
    {
        public long RequisitionStageConfigId { get; set; }

        public RequisitionStageConfigDeleteCommand(long requisitionStageConfigId)
        {
            RequisitionStageConfigId = requisitionStageConfigId;
        }
    }
}
