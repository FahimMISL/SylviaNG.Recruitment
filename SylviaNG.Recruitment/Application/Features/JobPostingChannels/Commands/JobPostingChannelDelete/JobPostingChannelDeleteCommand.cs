using MediatR;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelDelete
{
    public class JobPostingChannelDeleteCommand : IRequest<Unit>
    {
        public long JobPostingChannelId { get; set; }

        public JobPostingChannelDeleteCommand(long jobPostingChannelId)
        {
            JobPostingChannelId = jobPostingChannelId;
        }
    }
}
