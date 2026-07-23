using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationSubmit
{
    public class InterviewEvaluationSubmitValidator : AbstractValidator<InterviewEvaluationSubmitCommand>
    {
        public InterviewEvaluationSubmitValidator()
        {
            RuleFor(x => x.Request.EmployeeId)
                .GreaterThan(0).WithMessage("EmployeeId is required.");

            RuleFor(x => x.Request.ScorecardId)
                .GreaterThan(0).WithMessage("ScorecardId is required.");

            RuleFor(x => x.Request.Scores)
                .NotEmpty().WithMessage("At least one score is required.");

            RuleFor(x => x.Request.OverallComments)
                .MaximumLength(2000).WithMessage("Overall comments must not exceed 2000 characters.");
        }
    }
}
