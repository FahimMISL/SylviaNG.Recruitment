using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDelete
{
    public class HiringPipelineDeleteHandler : IRequestHandler<HiringPipelineDeleteCommand, Unit>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineDeleteHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<Unit> Handle(HiringPipelineDeleteCommand command, CancellationToken cancellationToken)
        {
            await _hiringPipelineService.DeleteAsync(command.HiringPipelineId);
            return Unit.Value;
        }
    }
}
