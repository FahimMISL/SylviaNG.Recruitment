using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateUpdate
{
    public class DocumentTemplateUpdateValidator : AbstractValidator<DocumentTemplateUpdateCommand>
    {
        public DocumentTemplateUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
