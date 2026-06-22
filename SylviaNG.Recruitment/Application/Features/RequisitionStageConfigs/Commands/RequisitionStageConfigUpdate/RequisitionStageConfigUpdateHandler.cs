using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigUpdate
{
    public class RequisitionStageConfigUpdateHandler : IRequestHandler<RequisitionStageConfigUpdateCommand, Unit>
    {
        private readonly IRequisitionStageConfigService _service;

        public RequisitionStageConfigUpdateHandler(IRequisitionStageConfigService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionStageConfigUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.RequisitionStageConfigId, command.Request);
            return Unit.Value;
        }
    }
}
