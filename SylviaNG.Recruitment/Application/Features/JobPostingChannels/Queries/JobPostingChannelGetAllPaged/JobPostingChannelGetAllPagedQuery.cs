using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostingChannels.Queries.JobPostingChannelGetAllPaged
{
    public class JobPostingChannelGetAllPagedQuery : IRequest<PagedResult<JobPostingChannelResponse>>
    {
        public PagedRequest Request { get; set; }

        public JobPostingChannelGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
