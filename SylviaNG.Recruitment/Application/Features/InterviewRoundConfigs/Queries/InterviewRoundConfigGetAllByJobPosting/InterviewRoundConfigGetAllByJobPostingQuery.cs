using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Queries.InterviewRoundConfigGetAllByJobPosting
{
    public class InterviewRoundConfigGetAllByJobPostingQuery : IRequest<List<InterviewRoundConfigResponse>>
    {
        public long JobPostingId { get; set; }

        public InterviewRoundConfigGetAllByJobPostingQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
