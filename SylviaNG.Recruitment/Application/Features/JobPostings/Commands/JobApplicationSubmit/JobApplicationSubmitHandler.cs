using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit
{
    public class JobApplicationSubmitHandler : IRequestHandler<JobApplicationSubmitCommand, JobApplicationResponse>
    {
        private readonly IJobApplicationSubmissionService _submissionService;

        public JobApplicationSubmitHandler(IJobApplicationSubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task<JobApplicationResponse> Handle(JobApplicationSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _submissionService.SubmitAsync(command.Request, command.Source);
        }
    }
}
