using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterDecline
{
    public class OfferLetterDeclineValidator : AbstractValidator<OfferLetterDeclineCommand>
    {
        public OfferLetterDeclineValidator()
        {
            RuleFor(x => x.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");

            // AC4: declining requires a mandatory reason.
            RuleFor(x => x.Request.Reason)
                .NotEmpty().WithMessage("A reason is required to decline an offer.")
                .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters.");
        }
    }
}
