using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPublicPaged
{
    public class JobPostingGetAllPublicPagedHandler : IRequestHandler<JobPostingGetAllPublicPagedQuery, PagedResult<JobPostingResponse>>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetAllPublicPagedHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<PagedResult<JobPostingResponse>> Handle(JobPostingGetAllPublicPagedQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetPaginatedPublicAsync(
                query.Request, query.Location, query.DepartmentId, query.EmploymentType, query.MaxExperienceYears);
        }
    }
}
