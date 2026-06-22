using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaUpdate
{
    public class InterviewScorecardCriteriaUpdateCommand : IRequest<Unit>
    {
        public long InterviewScorecardCriteriaId { get; set; }
        public InterviewScorecardCriteriaUpdateRequest Request { get; set; }

        public InterviewScorecardCriteriaUpdateCommand(long interviewScorecardCriteriaId, InterviewScorecardCriteriaUpdateRequest request)
        {
            InterviewScorecardCriteriaId = interviewScorecardCriteriaId;
            Request = request;
        }
    }
}
