using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelUpdate
{
    public class JobPostingChannelUpdateCommand : IRequest<Unit>
    {
        public long JobPostingChannelId { get; set; }
        public JobPostingChannelUpdateRequest Request { get; set; }

        public JobPostingChannelUpdateCommand(long jobPostingChannelId, JobPostingChannelUpdateRequest request)
        {
            JobPostingChannelId = jobPostingChannelId;
            Request = request;
        }
    }
}
