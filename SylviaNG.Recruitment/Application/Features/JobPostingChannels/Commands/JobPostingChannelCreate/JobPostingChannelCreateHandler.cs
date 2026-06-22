using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelCreate
{
    public class JobPostingChannelCreateHandler : IRequestHandler<JobPostingChannelCreateCommand, long>
    {
        private readonly IJobPostingChannelService _service;

        public JobPostingChannelCreateHandler(IJobPostingChannelService service)
        {
            _service = service;
        }

        public async Task<long> Handle(JobPostingChannelCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
