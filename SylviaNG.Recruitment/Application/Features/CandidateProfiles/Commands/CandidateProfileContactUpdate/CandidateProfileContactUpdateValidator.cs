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

            RuleFor(x => x.Request.Phone)
                .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.")
                .Matches(@"^[0-9+\-\s()]+$").WithMessage("Phone must be a valid phone number.")
                .When(x => !string.IsNullOrEmpty(x.Request.Phone));

            RuleFor(x => x.Request.PresentAddress)
                .MaximumLength(500).WithMessage("PresentAddress must not exceed 500 characters.");

            RuleFor(x => x.Request.PermanentAddress)
                .MaximumLength(500).WithMessage("PermanentAddress must not exceed 500 characters.");
        }
    }
}
