using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigCreate
{
    public class RequisitionStageConfigCreateHandler : IRequestHandler<RequisitionStageConfigCreateCommand, long>
    {
        private readonly IRequisitionStageConfigService _service;

        public RequisitionStageConfigCreateHandler(IRequisitionStageConfigService service)
        {
            _service = service;
        }

        public async Task<long> Handle(RequisitionStageConfigCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
