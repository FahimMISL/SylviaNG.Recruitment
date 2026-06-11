using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingCreate
{
    public class JobPostingCreateHandler : IRequestHandler<JobPostingCreateCommand, long>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingCreateHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<long> Handle(JobPostingCreateCommand command, CancellationToken cancellationToken)
        {
            return await _jobPostingService.CreateAsync(command.Request);
        }
    }
}
