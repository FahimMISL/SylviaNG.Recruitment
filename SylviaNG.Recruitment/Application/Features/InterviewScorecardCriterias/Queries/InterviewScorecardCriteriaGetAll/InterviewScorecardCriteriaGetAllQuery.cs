using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetAll
{
    public class InterviewScorecardCriteriaGetAllQuery : IRequest<List<InterviewScorecardCriteriaResponse>>
    {
    }
}
