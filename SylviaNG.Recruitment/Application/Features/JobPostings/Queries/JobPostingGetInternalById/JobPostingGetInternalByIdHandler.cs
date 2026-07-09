using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetInternalById
{
    public class JobPostingGetInternalByIdHandler : IRequestHandler<JobPostingGetInternalByIdQuery, JobPostingResponse>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetInternalByIdHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<JobPostingResponse> Handle(JobPostingGetInternalByIdQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetInternalByIdAsync(query.JobPostingId);
        }
    }
}
