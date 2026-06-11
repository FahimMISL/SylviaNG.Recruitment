using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetById
{
    public class JobPostingGetByIdQuery : IRequest<JobPostingResponse>
    {
        public long JobPostingId { get; set; }

        public JobPostingGetByIdQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
