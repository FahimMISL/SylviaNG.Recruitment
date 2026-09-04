using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetMyPostings
{
    public class JobPostingGetMyPostingsHandler : IRequestHandler<JobPostingGetMyPostingsQuery, List<JobPostingResponse>>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingGetMyPostingsHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<List<JobPostingResponse>> Handle(JobPostingGetMyPostingsQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingService.GetMyPostingsAsync();
        }
    }
}
