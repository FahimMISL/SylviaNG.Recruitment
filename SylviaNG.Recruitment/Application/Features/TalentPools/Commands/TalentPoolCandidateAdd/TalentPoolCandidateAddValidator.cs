using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCandidateAdd
{
    public class TalentPoolCandidateAddValidator : AbstractValidator<TalentPoolCandidateAddCommand>
    {
        public TalentPoolCandidateAddValidator()
        {
            RuleFor(x => x.TalentPoolId)
                .GreaterThan(0).WithMessage("TalentPoolId is required.");

            RuleFor(x => x.Request.CandidateProfileIds)
                .NotEmpty().WithMessage("At least one CandidateProfileId is required.");
        }
    }
}
