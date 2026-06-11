using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetById
{
    public class JobPostingGetByIdHandler : IRequestHandler<JobPostingGetByIdQuery, JobPostingResponse>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetByIdHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<JobPostingResponse> Handle(JobPostingGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetByIdAsync(query.JobPostingId);
        }
    }
}
