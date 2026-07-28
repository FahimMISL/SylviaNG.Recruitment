using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceCreate
{
    public class ReferralSourceCreateValidator : AbstractValidator<ReferralSourceCreateCommand>
    {
        public ReferralSourceCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Referral source name is required.")
                .MaximumLength(100).WithMessage("Referral source name must not exceed 100 characters.");
        }
    }
}
