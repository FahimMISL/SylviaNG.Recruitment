using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterAccept
{
    public class OfferLetterAcceptValidator : AbstractValidator<OfferLetterAcceptCommand>
    {
        public OfferLetterAcceptValidator()
        {
            RuleFor(x => x.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");
        }
    }
}
