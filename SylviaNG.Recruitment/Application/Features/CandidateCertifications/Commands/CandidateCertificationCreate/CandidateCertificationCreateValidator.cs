using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Commands.CandidateCertificationCreate
{
    public class CandidateCertificationCreateValidator : AbstractValidator<CandidateCertificationCreateCommand>
    {
        public CandidateCertificationCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
