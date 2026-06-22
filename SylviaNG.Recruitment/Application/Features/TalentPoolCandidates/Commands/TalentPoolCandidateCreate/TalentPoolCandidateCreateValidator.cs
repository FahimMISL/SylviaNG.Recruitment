using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateCreate
{
    public class TalentPoolCandidateCreateValidator : AbstractValidator<TalentPoolCandidateCreateCommand>
    {
        public TalentPoolCandidateCreateValidator()
        {
            RuleFor(x => x.Request.TalentPoolId).GreaterThan(0).WithMessage("TalentPoolId is required.");
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
