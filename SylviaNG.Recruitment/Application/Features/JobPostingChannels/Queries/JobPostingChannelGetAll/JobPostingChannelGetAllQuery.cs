using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAll
{
    public class JobPostingChannelGetAllQuery : IRequest<List<JobPostingChannelResponse>>
    {
    }
}
