using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletGenerate
{
    public class JoiningBookletGenerateValidator : AbstractValidator<JoiningBookletGenerateCommand>
    {
        public JoiningBookletGenerateValidator()
        {
            RuleFor(x => x.Request.OfferLetterId)
                .GreaterThan(0).WithMessage("OfferLetterId is required.");

            RuleFor(x => x.Request.DocumentTemplateId)
                .GreaterThan(0).WithMessage("DocumentTemplateId is required.");

            RuleFor(x => x.Request.BatchLabel)
                .NotEmpty().WithMessage("BatchLabel is required.");

            RuleFor(x => x.Request.JoiningDate)
                .NotEqual(default(DateTime)).WithMessage("JoiningDate is required.");
        }
    }
}
