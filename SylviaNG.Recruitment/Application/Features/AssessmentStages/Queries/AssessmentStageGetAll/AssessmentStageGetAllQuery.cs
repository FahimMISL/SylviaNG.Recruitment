using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAll
{
    public class AssessmentStageGetAllQuery : IRequest<List<AssessmentStageResponse>>
    {
    }
}
