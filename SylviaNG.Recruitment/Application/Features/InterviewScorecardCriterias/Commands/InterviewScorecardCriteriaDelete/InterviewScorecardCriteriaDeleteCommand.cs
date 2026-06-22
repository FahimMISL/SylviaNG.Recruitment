using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaDelete
{
    public class InterviewScorecardCriteriaDeleteCommand : IRequest<Unit>
    {
        public long InterviewScorecardCriteriaId { get; set; }

        public InterviewScorecardCriteriaDeleteCommand(long interviewScorecardCriteriaId)
        {
            InterviewScorecardCriteriaId = interviewScorecardCriteriaId;
        }
    }
}
