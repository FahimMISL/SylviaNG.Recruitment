using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreCreate
{
    public class InterviewEvaluationScoreCreateValidator : AbstractValidator<InterviewEvaluationScoreCreateCommand>
    {
        public InterviewEvaluationScoreCreateValidator()
        {
            RuleFor(x => x.Request.InterviewEvaluationId).GreaterThan(0).WithMessage("InterviewEvaluationId is required.");
        }
    }
}
