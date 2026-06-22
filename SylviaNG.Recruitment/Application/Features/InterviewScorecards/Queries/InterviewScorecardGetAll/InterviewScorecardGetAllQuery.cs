using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Queries.InterviewScorecardGetAll
{
    public class InterviewScorecardGetAllQuery : IRequest<List<InterviewScorecardResponse>>
    {
    }
}
