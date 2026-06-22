using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetById
{
    public class JobPostingChannelGetByIdHandler : IRequestHandler<JobPostingChannelGetByIdQuery, JobPostingChannelResponse>
    {
        private readonly IJobPostingChannelService _service;

        public JobPostingChannelGetByIdHandler(IJobPostingChannelService service)
        {
            _service = service;
        }

        public async Task<JobPostingChannelResponse> Handle(JobPostingChannelGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.JobPostingChannelId);
        }
    }
}
