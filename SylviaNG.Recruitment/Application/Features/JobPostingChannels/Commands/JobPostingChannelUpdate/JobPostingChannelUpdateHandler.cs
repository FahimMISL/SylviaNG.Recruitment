using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelUpdate
{
    public class JobPostingChannelUpdateHandler : IRequestHandler<JobPostingChannelUpdateCommand, Unit>
    {
        private readonly IJobPostingChannelService _service;

        public JobPostingChannelUpdateHandler(IJobPostingChannelService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(JobPostingChannelUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.JobPostingChannelId, command.Request);
            return Unit.Value;
        }
    }
}
