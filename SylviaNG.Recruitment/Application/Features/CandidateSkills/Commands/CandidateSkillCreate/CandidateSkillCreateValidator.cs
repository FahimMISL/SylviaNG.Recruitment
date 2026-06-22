using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateSkills.Commands.CandidateSkillCreate
{
    public class CandidateSkillCreateValidator : AbstractValidator<CandidateSkillCreateCommand>
    {
        public CandidateSkillCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
            RuleFor(x => x.Request.SkillName).NotEmpty().WithMessage("SkillName is required.").MaximumLength(200);
        }
    }
}
