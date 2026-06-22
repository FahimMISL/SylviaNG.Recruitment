using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Queries.InterviewScorecardCriteriaGetById
{
    public class InterviewScorecardCriteriaGetByIdQuery : IRequest<InterviewScorecardCriteriaResponse>
    {
        public long InterviewScorecardCriteriaId { get; set; }

        public InterviewScorecardCriteriaGetByIdQuery(long interviewScorecardCriteriaId)
        {
            InterviewScorecardCriteriaId = interviewScorecardCriteriaId;
        }
    }
}
