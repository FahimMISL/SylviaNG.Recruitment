using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaCreate
{
    public class InterviewScorecardCriteriaCreateCommand : IRequest<long>
    {
        public InterviewScorecardCriteriaCreateRequest Request { get; set; }

        public InterviewScorecardCriteriaCreateCommand(InterviewScorecardCriteriaCreateRequest request)
        {
            Request = request;
        }
    }
}
