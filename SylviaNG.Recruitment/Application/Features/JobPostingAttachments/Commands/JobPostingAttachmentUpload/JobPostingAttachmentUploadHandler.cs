using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentUpload
{
    public class JobPostingAttachmentUploadHandler : IRequestHandler<JobPostingAttachmentUploadCommand, JobPostingAttachmentResponse>
    {
        private readonly IJobPostingAttachmentService _jobPostingAttachmentService;

        public JobPostingAttachmentUploadHandler(IJobPostingAttachmentService jobPostingAttachmentService)
        {
            _jobPostingAttachmentService = jobPostingAttachmentService;
        }

        public async Task<JobPostingAttachmentResponse> Handle(JobPostingAttachmentUploadCommand command, CancellationToken cancellationToken)
        {
            return await _jobPostingAttachmentService.UploadAsync(command.JobPostingId, command.File);
        }
    }
}
