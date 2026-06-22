using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.DocumentAcceptances.Commands.DocumentAcceptanceCreate
{
    public class DocumentAcceptanceCreateValidator : AbstractValidator<DocumentAcceptanceCreateCommand>
    {
        public DocumentAcceptanceCreateValidator()
        {
            RuleFor(x => x.Request.GeneratedDocumentId).GreaterThan(0).WithMessage("GeneratedDocumentId is required.");
        }
    }
}
