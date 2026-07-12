using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceCreate
{
    public class CandidateWorkExperienceCreateValidator : AbstractValidator<CandidateWorkExperienceCreateCommand>
    {
        public CandidateWorkExperienceCreateValidator()
        {
            RuleFor(x => x.Request.CompanyName)
                .NotEmpty().WithMessage("CompanyName is required.")
                .MaximumLength(200).WithMessage("CompanyName must not exceed 200 characters.");

            RuleFor(x => x.Request.Designation)
                .NotEmpty().WithMessage("Designation is required.")
                .MaximumLength(200).WithMessage("Designation must not exceed 200 characters.");

            RuleFor(x => x.Request.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("StartDate must not be in the future.");

            RuleFor(x => x.Request.EndDate)
                .GreaterThan(x => x.Request.StartDate).WithMessage("EndDate must be after StartDate.")
                .When(x => x.Request.EndDate.HasValue);

            RuleFor(x => x.Request.EndDate)
                .Null().WithMessage("EndDate must not be set when IsCurrent is true.")
                .When(x => x.Request.IsCurrent);

            RuleFor(x => x.Request.Responsibilities)
                .NotEmpty().WithMessage("Responsibilities is required.");

            RuleFor(x => x.Request.Location)
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");
        }
    }
}
