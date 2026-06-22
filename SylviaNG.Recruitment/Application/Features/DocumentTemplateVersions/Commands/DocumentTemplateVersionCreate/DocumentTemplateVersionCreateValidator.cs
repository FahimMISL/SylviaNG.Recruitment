using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplateVersions.Commands.DocumentTemplateVersionCreate
{
    public class DocumentTemplateVersionCreateValidator : AbstractValidator<DocumentTemplateVersionCreateCommand>
    {
        public DocumentTemplateVersionCreateValidator()
        {
            RuleFor(x => x.Request.DocumentTemplateId).GreaterThan(0).WithMessage("DocumentTemplateId is required.");
        }
    }
}
