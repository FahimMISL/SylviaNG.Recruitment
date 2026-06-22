using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAllPaged
{
    public class JobPostingChannelGetAllPagedHandler : IRequestHandler<JobPostingChannelGetAllPagedQuery, PagedResult<JobPostingChannelResponse>>
    {
        private readonly IJobPostingChannelService _jobPostingChannelService;

        public JobPostingChannelGetAllPagedHandler(IJobPostingChannelService jobPostingChannelService)
        {
            _jobPostingChannelService = jobPostingChannelService;
        }

        public async Task<PagedResult<JobPostingChannelResponse>> Handle(JobPostingChannelGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingChannelService.GetPaginatedAsync(query.Request);
        }
    }
}
