using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Commands.HiringPipelineStageCreate
{
    public class HiringPipelineStageCreateCommand : IRequest<long>
    {
        public HiringPipelineStageCreateRequest Request { get; set; }

        public HiringPipelineStageCreateCommand(HiringPipelineStageCreateRequest request)
        {
            Request = request;
        }
    }
}
