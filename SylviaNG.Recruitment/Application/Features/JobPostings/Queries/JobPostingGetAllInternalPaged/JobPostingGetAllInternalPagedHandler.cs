using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllInternalPaged
{
    public class JobPostingGetAllInternalPagedHandler : IRequestHandler<JobPostingGetAllInternalPagedQuery, PagedResult<JobPostingResponse>>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetAllInternalPagedHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<PagedResult<JobPostingResponse>> Handle(JobPostingGetAllInternalPagedQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetPaginatedInternalAsync(
                query.Request, query.Location, query.DepartmentId, query.EmploymentType, query.MaxExperienceYears);
        }
    }
}
