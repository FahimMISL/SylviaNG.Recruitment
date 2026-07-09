using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetInternalById
{
    /// <summary>Internal job board detail view (any authenticated user) for a single Open, internally-visible job posting.</summary>
    public class JobPostingGetInternalByIdQuery : IRequest<JobPostingResponse>
    {
        public long JobPostingId { get; set; }

        public JobPostingGetInternalByIdQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
