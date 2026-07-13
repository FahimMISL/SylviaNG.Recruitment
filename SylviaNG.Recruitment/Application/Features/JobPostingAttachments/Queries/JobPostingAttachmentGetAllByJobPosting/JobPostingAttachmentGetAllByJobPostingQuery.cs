using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Queries.JobPostingAttachmentGetAllByJobPosting
{
    public class JobPostingAttachmentGetAllByJobPostingQuery : IRequest<List<JobPostingAttachmentResponse>>
    {
        public long JobPostingId { get; set; }

        public JobPostingAttachmentGetAllByJobPostingQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
