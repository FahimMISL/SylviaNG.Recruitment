using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentDelete
{
    public class JobPostingAttachmentDeleteHandler : IRequestHandler<JobPostingAttachmentDeleteCommand, Unit>
    {
        private readonly IJobPostingAttachmentService _jobPostingAttachmentService;

        public JobPostingAttachmentDeleteHandler(IJobPostingAttachmentService jobPostingAttachmentService)
        {
            _jobPostingAttachmentService = jobPostingAttachmentService;
        }

        public async Task<Unit> Handle(JobPostingAttachmentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _jobPostingAttachmentService.DeleteAsync(command.JobPostingId, command.JobPostingAttachmentId);
            return Unit.Value;
        }
    }
}
