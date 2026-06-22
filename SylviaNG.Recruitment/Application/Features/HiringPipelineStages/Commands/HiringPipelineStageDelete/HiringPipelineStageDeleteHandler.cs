using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageDelete
{
    public class HiringPipelineStageDeleteHandler : IRequestHandler<HiringPipelineStageDeleteCommand>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageDeleteHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task Handle(HiringPipelineStageDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.HiringPipelineStageId);
        }
    }
}
