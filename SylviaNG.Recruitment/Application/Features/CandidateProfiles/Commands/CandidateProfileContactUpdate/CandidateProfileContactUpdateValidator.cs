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

            // Local number typed after the candidate picks a Country (dial code) - length varies
            // by country, so this only guards against obviously-wrong input, not a fixed format.
            RuleFor(x => x.Request.Phone)
                .Matches(@"^[0-9]{6,15}$").WithMessage("Phone must be a valid mobile number.")
                .When(x => !string.IsNullOrEmpty(x.Request.Phone));

            RuleFor(x => x.Request.PresentAddressDetail)
                .MaximumLength(500).WithMessage("PresentAddressDetail must not exceed 500 characters.");

            RuleFor(x => x.Request.PermanentAddressDetail)
                .MaximumLength(500).WithMessage("PermanentAddressDetail must not exceed 500 characters.");
        }
    }
}
