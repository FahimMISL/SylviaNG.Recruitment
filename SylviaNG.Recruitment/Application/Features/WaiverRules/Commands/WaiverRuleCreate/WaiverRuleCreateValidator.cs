using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.WaiverRules.Commands.WaiverRuleCreate
{
    public class WaiverRuleCreateValidator : AbstractValidator<WaiverRuleCreateCommand>
    {
        public WaiverRuleCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Waiver rule name is required.")
                .MaximumLength(200).WithMessage("Waiver rule name must not exceed 200 characters.");

            RuleFor(x => x.Request.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Request.Priority)
                .GreaterThanOrEqualTo(0).WithMessage("Priority must be zero or a positive integer (lower evaluates first).");
        }
    }
}
