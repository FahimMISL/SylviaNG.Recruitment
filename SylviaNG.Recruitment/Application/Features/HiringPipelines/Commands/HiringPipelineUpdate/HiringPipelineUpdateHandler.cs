using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineUpdate
{
    public class HiringPipelineUpdateHandler : IRequestHandler<HiringPipelineUpdateCommand, Unit>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineUpdateHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<Unit> Handle(HiringPipelineUpdateCommand command, CancellationToken cancellationToken)
        {
            await _hiringPipelineService.UpdateAsync(command.HiringPipelineId, command.Request);
            return Unit.Value;
        }
    }
}
