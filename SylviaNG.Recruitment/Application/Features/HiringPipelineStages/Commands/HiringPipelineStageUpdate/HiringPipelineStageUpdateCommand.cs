using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageUpdate
{
    public class HiringPipelineStageUpdateCommand : IRequest
    {
        public long HiringPipelineStageId { get; set; }
        public HiringPipelineStageUpdateRequest Request { get; set; }

        public HiringPipelineStageUpdateCommand(long hiringPipelineStageId, HiringPipelineStageUpdateRequest request)
        {
            HiringPipelineStageId = hiringPipelineStageId;
            Request = request;
        }
    }
}
