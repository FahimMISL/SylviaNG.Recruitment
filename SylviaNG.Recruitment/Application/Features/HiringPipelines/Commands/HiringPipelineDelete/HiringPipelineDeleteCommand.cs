using MediatR;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDelete
{
    public class HiringPipelineDeleteCommand : IRequest<Unit>
    {
        public long HiringPipelineId { get; set; }

        public HiringPipelineDeleteCommand(long hiringPipelineId)
        {
            HiringPipelineId = hiringPipelineId;
        }
    }
}
