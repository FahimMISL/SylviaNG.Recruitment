using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineCreate
{
    public class HiringPipelineCreateHandler : IRequestHandler<HiringPipelineCreateCommand, long>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineCreateHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<long> Handle(HiringPipelineCreateCommand command, CancellationToken cancellationToken)
        {
            return await _hiringPipelineService.CreateAsync(command.Request);
        }
    }
}
