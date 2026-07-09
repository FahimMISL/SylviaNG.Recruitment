using MediatR;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineSetActive
{
    public class HiringPipelineSetActiveCommand : IRequest<Unit>
    {
        public long HiringPipelineId { get; set; }
        public bool IsActive { get; set; }

        public HiringPipelineSetActiveCommand(long hiringPipelineId, bool isActive)
        {
            HiringPipelineId = hiringPipelineId;
            IsActive = isActive;
        }
    }
}
