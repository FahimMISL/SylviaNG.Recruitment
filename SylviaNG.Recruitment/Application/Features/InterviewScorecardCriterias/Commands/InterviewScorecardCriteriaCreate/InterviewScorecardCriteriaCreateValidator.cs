using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Commands.InterviewScorecardCriteriaCreate
{
    public class InterviewScorecardCriteriaCreateValidator : AbstractValidator<InterviewScorecardCriteriaCreateCommand>
    {
        public InterviewScorecardCriteriaCreateValidator()
        {
            RuleFor(x => x.Request.InterviewScorecardId).GreaterThan(0).WithMessage("InterviewScorecardId is required.");
        }
    }
}
