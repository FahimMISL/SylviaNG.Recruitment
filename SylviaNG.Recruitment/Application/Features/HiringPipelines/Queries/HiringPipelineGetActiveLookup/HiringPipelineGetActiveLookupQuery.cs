using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelines.Models;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Queries.HiringPipelineGetActiveLookup
{
    public class HiringPipelineGetActiveLookupQuery : IRequest<List<HiringPipelineLookupResponse>>
    {
    }
}
