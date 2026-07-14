using MediatR;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineDuplicate
{
    public class HiringPipelineDuplicateCommand : IRequest<long>
    {
        public long HiringPipelineId { get; set; }

        public HiringPipelineDuplicateCommand(long hiringPipelineId)
        {
            HiringPipelineId = hiringPipelineId;
        }
    }
}
