using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationCreate
{
    public class InterviewEvaluationCreateValidator : AbstractValidator<InterviewEvaluationCreateCommand>
    {
        public InterviewEvaluationCreateValidator()
        {
            RuleFor(x => x.Request.InterviewId).GreaterThan(0).WithMessage("InterviewId is required.");
        }
    }
}
