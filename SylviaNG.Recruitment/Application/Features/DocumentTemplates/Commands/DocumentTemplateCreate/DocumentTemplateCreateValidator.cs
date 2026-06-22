using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateCreate
{
    public class DocumentTemplateCreateValidator : AbstractValidator<DocumentTemplateCreateCommand>
    {
        public DocumentTemplateCreateValidator()
        {
            RuleFor(x => x.Request.TemplateName)
                .NotEmpty().WithMessage("TemplateName is required.")
                .MaximumLength(500).WithMessage("TemplateName must not exceed 500 characters.");
        }
    }
}
