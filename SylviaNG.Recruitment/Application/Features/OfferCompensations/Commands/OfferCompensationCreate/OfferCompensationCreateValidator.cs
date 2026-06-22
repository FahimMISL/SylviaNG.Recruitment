using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationCreate
{
    public class OfferCompensationCreateValidator : AbstractValidator<OfferCompensationCreateCommand>
    {
        public OfferCompensationCreateValidator()
        {
            RuleFor(x => x.Request.FitmentDataId).GreaterThan(0).WithMessage("FitmentDataId is required.");
        }
    }
}
