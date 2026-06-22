using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigDelete
{
    public class RequisitionStageConfigDeleteHandler : IRequestHandler<RequisitionStageConfigDeleteCommand, Unit>
    {
        private readonly IRequisitionStageConfigService _service;

        public RequisitionStageConfigDeleteHandler(IRequisitionStageConfigService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(RequisitionStageConfigDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.RequisitionStageConfigId);
            return Unit.Value;
        }
    }
}
