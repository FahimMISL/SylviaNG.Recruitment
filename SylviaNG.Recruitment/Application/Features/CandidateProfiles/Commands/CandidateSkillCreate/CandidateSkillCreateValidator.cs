using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateSkillCreate
{
    public class CandidateSkillCreateValidator : AbstractValidator<CandidateSkillCreateCommand>
    {
        public CandidateSkillCreateValidator()
        {
            RuleFor(x => x.Request.SkillName)
                .NotEmpty().WithMessage("SkillName is required.")
                .MaximumLength(100).WithMessage("SkillName must not exceed 100 characters.");

            RuleFor(x => x.Request.ProficiencyLevel)
                .MaximumLength(20).WithMessage("ProficiencyLevel must not exceed 20 characters.");

            RuleFor(x => x.Request.SkillLibraryItemId)
                .GreaterThan(0).WithMessage("SkillLibraryItemId must be a positive id.")
                .When(x => x.Request.SkillLibraryItemId.HasValue);
        }
    }
}
