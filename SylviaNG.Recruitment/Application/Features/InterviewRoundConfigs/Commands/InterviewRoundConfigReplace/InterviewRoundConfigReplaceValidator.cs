using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Commands.InterviewRoundConfigReplace
{
    public class InterviewRoundConfigReplaceValidator : AbstractValidator<InterviewRoundConfigReplaceCommand>
    {
        public InterviewRoundConfigReplaceValidator()
        {
            RuleForEach(x => x.Request.Rounds).ChildRules(round =>
            {
                round.RuleFor(r => r.Name)
                    .NotEmpty().WithMessage("Round name is required.")
                    .MaximumLength(200).WithMessage("Round name must not exceed 200 characters.");

                round.RuleFor(r => r.Sequence)
                    .GreaterThan(0).WithMessage("Sequence must be greater than zero.");
            });
        }
    }
}
