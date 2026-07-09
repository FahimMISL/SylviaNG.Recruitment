using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetAll
{
    public class HiringPipelineGetAllQuery : IRequest<List<HiringPipelineResponse>>
    {
    }
}
