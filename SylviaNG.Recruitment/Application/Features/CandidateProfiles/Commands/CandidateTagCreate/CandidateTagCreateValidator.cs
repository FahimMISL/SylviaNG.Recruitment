using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateTagCreate
{
    public class CandidateTagCreateValidator : AbstractValidator<CandidateTagCreateCommand>
    {
        public CandidateTagCreateValidator()
        {
            RuleFor(x => x.CandidateProfileId)
                .GreaterThan(0).WithMessage("CandidateProfileId is required.");

            RuleFor(x => x.Request.TagName)
                .NotEmpty().WithMessage("TagName is required.")
                .MaximumLength(100).WithMessage("TagName must not exceed 100 characters.");
        }
    }
}
