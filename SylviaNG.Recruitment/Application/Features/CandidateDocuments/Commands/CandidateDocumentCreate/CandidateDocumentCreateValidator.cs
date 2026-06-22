using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateDocuments.Commands.CandidateDocumentCreate
{
    public class CandidateDocumentCreateValidator : AbstractValidator<CandidateDocumentCreateCommand>
    {
        public CandidateDocumentCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
