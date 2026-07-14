using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineUpdate
{
    public class HiringPipelineUpdateCommand : IRequest<Unit>
    {
        public long HiringPipelineId { get; set; }
        public HiringPipelineUpdateRequest Request { get; set; }

        public HiringPipelineUpdateCommand(long hiringPipelineId, HiringPipelineUpdateRequest request)
        {
            HiringPipelineId = hiringPipelineId;
            Request = request;
        }
    }
}
