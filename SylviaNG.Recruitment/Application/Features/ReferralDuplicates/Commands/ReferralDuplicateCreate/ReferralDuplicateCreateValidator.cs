using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Commands.ReferralDuplicateCreate
{
    public class ReferralDuplicateCreateValidator : AbstractValidator<ReferralDuplicateCreateCommand>
    {
        public ReferralDuplicateCreateValidator()
        {
            RuleFor(x => x.Request.PrimaryReferralId)
                .GreaterThan(0).WithMessage("PrimaryReferralId is required.");

            RuleFor(x => x.Request.MatchField)
                .NotEmpty().WithMessage("MatchField is required.")
                .MaximumLength(500).WithMessage("MatchField must not exceed 500 characters.");
        }
    }
}
