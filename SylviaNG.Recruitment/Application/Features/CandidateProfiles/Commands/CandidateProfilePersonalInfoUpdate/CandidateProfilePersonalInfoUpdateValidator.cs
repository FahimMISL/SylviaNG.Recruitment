using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePersonalInfoUpdate
{
    /// <summary>
    /// Validates the MediatR command (not the raw request DTO) - see JobApplicationSubmitValidator
    /// for the established precedent on why this app's FluentValidation auto-validation requires
    /// targeting the Command type.
    /// </summary>
    public class CandidateProfilePersonalInfoUpdateValidator : AbstractValidator<CandidateProfilePersonalInfoUpdateCommand>
    {
        public CandidateProfilePersonalInfoUpdateValidator()
        {
            RuleFor(x => x.Request.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

            RuleFor(x => x.Request.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("DateOfBirth must be in the past.")
                .When(x => x.Request.DateOfBirth.HasValue);

            RuleFor(x => x.Request.Gender)
                .IsInEnum().WithMessage("Gender must be a valid value.")
                .When(x => x.Request.Gender.HasValue);

            RuleFor(x => x.Request.NationalId)
                .MaximumLength(50).WithMessage("NationalId must not exceed 50 characters.");

            RuleFor(x => x.Request.FatherName)
                .MaximumLength(200).WithMessage("FatherName must not exceed 200 characters.");

            RuleFor(x => x.Request.MotherName)
                .MaximumLength(200).WithMessage("MotherName must not exceed 200 characters.");

            RuleFor(x => x.Request.MaritalStatus)
                .IsInEnum().WithMessage("MaritalStatus must be a valid value.")
                .When(x => x.Request.MaritalStatus.HasValue);

            RuleFor(x => x.Request.Religion)
                .IsInEnum().WithMessage("Religion must be a valid value.")
                .When(x => x.Request.Religion.HasValue);

            RuleFor(x => x.Request.Nationality)
                .MaximumLength(100).WithMessage("Nationality must not exceed 100 characters.");

            RuleFor(x => x.Request.BloodGroup)
                .IsInEnum().WithMessage("BloodGroup must be a valid value.")
                .When(x => x.Request.BloodGroup.HasValue);
        }
    }
}
