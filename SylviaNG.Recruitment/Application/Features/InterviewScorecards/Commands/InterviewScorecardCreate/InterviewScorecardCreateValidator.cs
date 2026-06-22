using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewScorecards.Commands.InterviewScorecardCreate
{
    public class InterviewScorecardCreateValidator : AbstractValidator<InterviewScorecardCreateCommand>
    {
        public InterviewScorecardCreateValidator()
        {
            RuleFor(x => x.Request.ScorecardName)
                .NotEmpty().WithMessage("ScorecardName is required.")
                .MaximumLength(500).WithMessage("ScorecardName must not exceed 500 characters.");
        }
    }
}
