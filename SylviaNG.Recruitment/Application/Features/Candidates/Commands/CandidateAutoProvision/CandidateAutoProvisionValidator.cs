using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateAutoProvision
{
    public class CandidateAutoProvisionValidator : AbstractValidator<CandidateAutoProvisionCommand>
    {
        public CandidateAutoProvisionValidator()
        {
            RuleFor(x => x.Request.KeycloakUserId).NotEmpty().WithMessage("Keycloak user ID is required.");
            RuleFor(x => x.Request.FullName).NotEmpty().WithMessage("Full name is required.").MaximumLength(200);
            RuleFor(x => x.Request.Email).NotEmpty().WithMessage("Email is required.").EmailAddress();
        }
    }
}
