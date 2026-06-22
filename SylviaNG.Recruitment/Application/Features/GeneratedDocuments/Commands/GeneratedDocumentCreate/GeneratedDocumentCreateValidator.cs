using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentCreate
{
    public class GeneratedDocumentCreateValidator : AbstractValidator<GeneratedDocumentCreateCommand>
    {
        public GeneratedDocumentCreateValidator()
        {
            RuleFor(x => x.Request.DocumentTemplateId).GreaterThan(0).WithMessage("DocumentTemplateId is required.");
        }
    }
}
