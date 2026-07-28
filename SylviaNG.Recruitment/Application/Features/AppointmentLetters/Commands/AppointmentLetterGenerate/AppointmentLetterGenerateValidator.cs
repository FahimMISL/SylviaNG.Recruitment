using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AppointmentLetters.Commands.AppointmentLetterGenerate
{
    public class AppointmentLetterGenerateValidator : AbstractValidator<AppointmentLetterGenerateCommand>
    {
        public AppointmentLetterGenerateValidator()
        {
            RuleFor(x => x.Request.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");

            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.FinalBody)
                .NotEmpty().WithMessage("FinalBody is required.");
        }
    }
}
