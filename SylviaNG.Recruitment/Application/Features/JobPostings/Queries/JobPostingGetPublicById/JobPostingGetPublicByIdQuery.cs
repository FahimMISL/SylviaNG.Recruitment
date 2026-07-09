using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetPublicById
{
    /// <summary>Anonymous career-portal detail view for a single Open, externally-visible job posting.</summary>
    public class JobPostingGetPublicByIdQuery : IRequest<JobPostingResponse>
    {
        public long JobPostingId { get; set; }

        public JobPostingGetPublicByIdQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
