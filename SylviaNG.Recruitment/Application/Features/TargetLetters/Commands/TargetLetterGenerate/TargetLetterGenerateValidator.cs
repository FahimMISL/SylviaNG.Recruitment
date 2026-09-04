using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Commands.TargetLetterGenerate
{
    public class TargetLetterGenerateValidator : AbstractValidator<TargetLetterGenerateCommand>
    {
        public TargetLetterGenerateValidator()
        {
            RuleFor(x => x.Request.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");

            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.Kpis)
                .NotEmpty().WithMessage("Kpis is required.");

            RuleFor(x => x.Request.Objectives)
                .NotEmpty().WithMessage("Objectives is required.");

            RuleFor(x => x.Request.FinalBody)
                .NotEmpty().WithMessage("FinalBody is required.");
        }
    }
}
