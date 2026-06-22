using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetById
{
    public class JobPostingChannelGetByIdQuery : IRequest<JobPostingChannelResponse>
    {
        public long JobPostingChannelId { get; set; }

        public JobPostingChannelGetByIdQuery(long jobPostingChannelId)
        {
            JobPostingChannelId = jobPostingChannelId;
        }
    }
}
