using MediatR;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobPostingUpdate
{
    public class JobPostingUpdateCommand : IRequest<Unit>
    {
        public long JobPostingId { get; set; }
        public JobPostingUpdateRequest Request { get; set; }

        public JobPostingUpdateCommand(long jobPostingId, JobPostingUpdateRequest request)
        {
            JobPostingId = jobPostingId;
            Request = request;
        }
    }
}
