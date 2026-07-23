using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileContactUpdate
{
    public class CandidateProfileContactUpdateValidator : AbstractValidator<CandidateProfileContactUpdateCommand>
    {
        public CandidateProfileContactUpdateValidator()
        {
            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");

            // Standard 11-digit "01" + operator-prefix format, or a plain 8-digit number when the
            // candidate picked the "Other" operator (no fixed 3-digit prefix to compose with).
            RuleFor(x => x.Request.Phone)
                .Matches(@"^(01[0-9]{9}|[0-9]{8})$").WithMessage("Phone must be a valid mobile number.")
                .When(x => !string.IsNullOrEmpty(x.Request.Phone));

            RuleFor(x => x.Request.MobileOperator)
                .IsInEnum().WithMessage("MobileOperator must be a valid value.")
                .When(x => x.Request.MobileOperator.HasValue);

            RuleFor(x => x.Request.PresentAddressDetail)
                .MaximumLength(500).WithMessage("PresentAddressDetail must not exceed 500 characters.");

            RuleFor(x => x.Request.PermanentAddressDetail)
                .MaximumLength(500).WithMessage("PermanentAddressDetail must not exceed 500 characters.");
        }
    }
}
