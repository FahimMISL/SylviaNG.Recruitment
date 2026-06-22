using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAll
{
    public class JobPostingChannelGetAllHandler : IRequestHandler<JobPostingChannelGetAllQuery, List<JobPostingChannelResponse>>
    {
        private readonly IJobPostingChannelService _service;

        public JobPostingChannelGetAllHandler(IJobPostingChannelService service)
        {
            _service = service;
        }

        public async Task<List<JobPostingChannelResponse>> Handle(JobPostingChannelGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
