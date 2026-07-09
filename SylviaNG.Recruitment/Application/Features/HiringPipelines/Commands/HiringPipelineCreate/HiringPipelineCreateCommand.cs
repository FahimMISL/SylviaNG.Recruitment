using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineCreate
{
    public class HiringPipelineCreateCommand : IRequest<long>
    {
        public HiringPipelineCreateRequest Request { get; set; }

        public HiringPipelineCreateCommand(HiringPipelineCreateRequest request)
        {
            Request = request;
        }
    }
}
