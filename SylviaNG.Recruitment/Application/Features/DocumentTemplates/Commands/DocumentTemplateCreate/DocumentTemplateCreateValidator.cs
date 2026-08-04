using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate
{
    public class DocumentTemplateCreateValidator : AbstractValidator<DocumentTemplateCreateCommand>
    {
        public DocumentTemplateCreateValidator()
        {
            RuleFor(x => x.Request.DocumentType)
                .IsInEnum().WithMessage("A valid DocumentType is required.");

            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(100).WithMessage("Code must not exceed 100 characters.");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
