using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetById
{
    public class HiringPipelineGetByIdQuery : IRequest<HiringPipelineResponse>
    {
        public long HiringPipelineId { get; set; }

        public HiringPipelineGetByIdQuery(long hiringPipelineId)
        {
            HiringPipelineId = hiringPipelineId;
        }
    }
}
