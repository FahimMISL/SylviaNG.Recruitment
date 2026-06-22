using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageCreate
{
    public class HiringPipelineStageCreateHandler : IRequestHandler<HiringPipelineStageCreateCommand, long>
    {
        private readonly IHiringPipelineStageService _service;

        public HiringPipelineStageCreateHandler(IHiringPipelineStageService service)
        {
            _service = service;
        }

        public async Task<long> Handle(HiringPipelineStageCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
