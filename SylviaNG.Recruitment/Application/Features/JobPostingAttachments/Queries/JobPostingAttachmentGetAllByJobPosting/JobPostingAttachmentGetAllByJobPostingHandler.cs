using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Queries.JobPostingAttachmentGetAllByJobPosting
{
    public class JobPostingAttachmentGetAllByJobPostingHandler
        : IRequestHandler<JobPostingAttachmentGetAllByJobPostingQuery, List<JobPostingAttachmentResponse>>
    {
        private readonly IJobPostingAttachmentService _jobPostingAttachmentService;

        public JobPostingAttachmentGetAllByJobPostingHandler(IJobPostingAttachmentService jobPostingAttachmentService)
        {
            _jobPostingAttachmentService = jobPostingAttachmentService;
        }

        public async Task<List<JobPostingAttachmentResponse>> Handle(
            JobPostingAttachmentGetAllByJobPostingQuery query, CancellationToken cancellationToken)
        {
            return await _jobPostingAttachmentService.GetAllByJobPostingIdAsync(query.JobPostingId);
        }
    }
}
