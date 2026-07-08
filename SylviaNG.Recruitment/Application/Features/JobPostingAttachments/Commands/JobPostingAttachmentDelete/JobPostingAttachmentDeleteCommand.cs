using MediatR;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentDelete
{
    public class JobPostingAttachmentDeleteCommand : IRequest<Unit>
    {
        public long JobPostingId { get; set; }
        public long JobPostingAttachmentId { get; set; }

        public JobPostingAttachmentDeleteCommand(long jobPostingId, long jobPostingAttachmentId)
        {
            JobPostingId = jobPostingId;
            JobPostingAttachmentId = jobPostingAttachmentId;
        }
    }
}
