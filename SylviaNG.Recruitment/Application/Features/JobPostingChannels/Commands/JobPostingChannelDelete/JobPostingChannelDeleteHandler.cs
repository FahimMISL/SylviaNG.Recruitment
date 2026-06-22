using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelDelete
{
    public class JobPostingChannelDeleteHandler : IRequestHandler<JobPostingChannelDeleteCommand, Unit>
    {
        private readonly IJobPostingChannelService _service;

        public JobPostingChannelDeleteHandler(IJobPostingChannelService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(JobPostingChannelDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.JobPostingChannelId);
            return Unit.Value;
        }
    }
}
