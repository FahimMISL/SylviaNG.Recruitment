using MediatR;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentUpload
{
    public class JobPostingAttachmentUploadCommand : IRequest<JobPostingAttachmentResponse>
    {
        public long JobPostingId { get; set; }
        public IFormFile File { get; set; }

        public JobPostingAttachmentUploadCommand(long jobPostingId, IFormFile file)
        {
            JobPostingId = jobPostingId;
            File = file;
        }
    }
}
