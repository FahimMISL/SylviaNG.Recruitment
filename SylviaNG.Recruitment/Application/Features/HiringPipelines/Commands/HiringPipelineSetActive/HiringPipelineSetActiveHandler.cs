using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineSetActive
{
    public class HiringPipelineSetActiveHandler : IRequestHandler<HiringPipelineSetActiveCommand, Unit>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineSetActiveHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<Unit> Handle(HiringPipelineSetActiveCommand command, CancellationToken cancellationToken)
        {
            await _hiringPipelineService.SetActiveAsync(command.HiringPipelineId, command.IsActive);
            return Unit.Value;
        }
    }
}
