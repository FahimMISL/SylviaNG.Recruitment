using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDuplicate
{
    public class HiringPipelineDuplicateHandler : IRequestHandler<HiringPipelineDuplicateCommand, long>
    {
        private readonly IHiringPipelineService _hiringPipelineService;

        public HiringPipelineDuplicateHandler(IHiringPipelineService hiringPipelineService)
        {
            _hiringPipelineService = hiringPipelineService;
        }

        public async Task<long> Handle(HiringPipelineDuplicateCommand command, CancellationToken cancellationToken)
        {
            return await _hiringPipelineService.DuplicateAsync(command.HiringPipelineId);
        }
    }
}
