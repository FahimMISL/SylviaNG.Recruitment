using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MedicalLetters.Commands.MedicalLetterGenerate
{
    public class MedicalLetterGenerateValidator : AbstractValidator<MedicalLetterGenerateCommand>
    {
        public MedicalLetterGenerateValidator()
        {
            RuleFor(x => x.Request.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");

            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.MedicalTestCenter)
                .NotEmpty().WithMessage("MedicalTestCenter is required.");

            RuleFor(x => x.Request.RequiredTests)
                .NotEmpty().WithMessage("RequiredTests is required.");

            RuleFor(x => x.Request.FinalBody)
                .NotEmpty().WithMessage("FinalBody is required.");
        }
    }
}
