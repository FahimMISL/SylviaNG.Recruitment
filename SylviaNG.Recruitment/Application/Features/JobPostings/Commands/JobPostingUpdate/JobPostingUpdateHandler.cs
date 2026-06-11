using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingUpdate
{
    public class JobPostingUpdateHandler : IRequestHandler<JobPostingUpdateCommand, Unit>
    {
        private readonly IJobPostingService _jobPostingService;

        public JobPostingUpdateHandler(IJobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }

        public async Task<Unit> Handle(JobPostingUpdateCommand command, CancellationToken cancellationToken)
        {
            await _jobPostingService.UpdateAsync(command.JobPostingId, command.Request);
            return Unit.Value;
        }
    }
}
