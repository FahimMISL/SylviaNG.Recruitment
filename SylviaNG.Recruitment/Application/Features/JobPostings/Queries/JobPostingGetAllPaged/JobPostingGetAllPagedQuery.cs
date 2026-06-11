using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPaged
{
    public class JobPostingGetAllPagedQuery : IRequest<PagedResult<JobPostingResponse>>
    {
        public PagedRequest Request { get; set; }

        public JobPostingGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
