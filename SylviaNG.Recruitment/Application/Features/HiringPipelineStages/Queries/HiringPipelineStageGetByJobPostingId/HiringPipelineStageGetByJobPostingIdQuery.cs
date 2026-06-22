using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetByJobPostingId
{
    public class HiringPipelineStageGetByJobPostingIdQuery : IRequest<List<HiringPipelineStageResponse>>
    {
        public long JobPostingId { get; set; }

        public HiringPipelineStageGetByJobPostingIdQuery(long jobPostingId)
        {
            JobPostingId = jobPostingId;
        }
    }
}
