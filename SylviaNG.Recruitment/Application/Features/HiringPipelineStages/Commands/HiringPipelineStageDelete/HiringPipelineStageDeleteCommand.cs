using MediatR;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageDelete
{
    public class HiringPipelineStageDeleteCommand : IRequest
    {
        public long HiringPipelineStageId { get; set; }

        public HiringPipelineStageDeleteCommand(long hiringPipelineStageId)
        {
            HiringPipelineStageId = hiringPipelineStageId;
        }
    }
}
