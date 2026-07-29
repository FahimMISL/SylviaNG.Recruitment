using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate
{
    public class InterviewEvaluationUpdateValidator : AbstractValidator<InterviewEvaluationUpdateCommand>
    {
        public InterviewEvaluationUpdateValidator()
        {
            RuleFor(x => x.Request.Scores)
                .NotEmpty().WithMessage("At least one score is required.");

            RuleFor(x => x.Request.OverallComments)
                .MaximumLength(2000).WithMessage("Overall comments must not exceed 2000 characters.");
        }
    }
}
