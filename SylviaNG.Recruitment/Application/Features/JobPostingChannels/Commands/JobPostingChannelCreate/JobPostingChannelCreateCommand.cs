using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Commands.JobPostingChannelCreate
{
    public class JobPostingChannelCreateCommand : IRequest<long>
    {
        public JobPostingChannelCreateRequest Request { get; set; }

        public JobPostingChannelCreateCommand(JobPostingChannelCreateRequest request)
        {
            Request = request;
        }
    }
}
