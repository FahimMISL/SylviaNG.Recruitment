using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetById
{
    public class HiringPipelineStageGetByIdQuery : IRequest<HiringPipelineStageResponse>
    {
        public long HiringPipelineStageId { get; set; }

        public HiringPipelineStageGetByIdQuery(long hiringPipelineStageId)
        {
            HiringPipelineStageId = hiringPipelineStageId;
        }
    }
}
