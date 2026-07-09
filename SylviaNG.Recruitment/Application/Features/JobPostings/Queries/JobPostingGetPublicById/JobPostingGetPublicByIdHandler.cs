using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetPublicById
{
    public class JobPostingGetPublicByIdHandler : IRequestHandler<JobPostingGetPublicByIdQuery, JobPostingResponse>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetPublicByIdHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<JobPostingResponse> Handle(JobPostingGetPublicByIdQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetPublicByIdAsync(query.JobPostingId);
        }
    }
}
