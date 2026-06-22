using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Referrals.Commands.ReferralCreate
{
    public class ReferralCreateValidator : AbstractValidator<ReferralCreateCommand>
    {
        public ReferralCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
