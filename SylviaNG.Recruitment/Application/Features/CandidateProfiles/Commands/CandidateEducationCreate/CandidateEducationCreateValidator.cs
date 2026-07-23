using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateEducationCreate
{
    public class CandidateEducationCreateValidator : AbstractValidator<CandidateEducationCreateCommand>
    {
        public CandidateEducationCreateValidator()
        {
            RuleFor(x => x.Request.DegreeId)
                .GreaterThan(0).WithMessage("Degree is required.");

            RuleFor(x => x.Request.Institution)
                .NotEmpty().WithMessage("Institution is required.")
                .MaximumLength(200).WithMessage("Institution must not exceed 200 characters.");

            RuleFor(x => x.Request.PassingYear)
                .InclusiveBetween(1950, DateTime.UtcNow.Year).WithMessage($"PassingYear must be between 1950 and {DateTime.UtcNow.Year}.");

            RuleFor(x => x.Request.GradingSystem)
                .IsInEnum().WithMessage("GradingSystem must be a valid value.")
                .When(x => x.Request.GradingSystem.HasValue);

            RuleFor(x => x.Request.Result)
                .NotEmpty().WithMessage("Result is required.")
                .MaximumLength(50).WithMessage("Result must not exceed 50 characters.");

            RuleFor(x => x.Request.MajorSubjectOtherText)
                .MaximumLength(200).WithMessage("MajorSubjectOtherText must not exceed 200 characters.");
        }
    }
}
