using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAll
{
    public class HiringPipelineStageGetAllQuery : IRequest<List<HiringPipelineStageResponse>>
    {
    }
}
