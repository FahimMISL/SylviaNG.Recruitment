using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit
{
    public class JobApplicationSubmitHandler : IRequestHandler<JobApplicationSubmitCommand, JobApplicationResponse>
    {
        private readonly IJobApplicationService _jobApplicationService;

        public JobApplicationSubmitHandler(IJobApplicationService jobApplicationService)
        {
            _jobApplicationService = jobApplicationService;
        }

        public async Task<JobApplicationResponse> Handle(JobApplicationSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _jobApplicationService.SubmitAsync(command.Request, command.Source);
        }
    }
}
