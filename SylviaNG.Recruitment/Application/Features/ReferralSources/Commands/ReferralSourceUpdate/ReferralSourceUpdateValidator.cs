using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ReferralSources.Commands.ReferralSourceUpdate
{
    public class ReferralSourceUpdateValidator : AbstractValidator<ReferralSourceUpdateCommand>
    {
        public ReferralSourceUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Referral source name is required.")
                .MaximumLength(100).WithMessage("Referral source name must not exceed 100 characters.");
        }
    }
}
