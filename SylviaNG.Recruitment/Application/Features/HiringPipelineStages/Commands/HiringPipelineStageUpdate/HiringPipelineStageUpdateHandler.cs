using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageUpdate
{
    public class HiringPipelineStageUpdateHandler : IRequestHandler<HiringPipelineStageUpdateCommand>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageUpdateHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task Handle(HiringPipelineStageUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.HiringPipelineStageId, command.Request);
        }
    }
}
