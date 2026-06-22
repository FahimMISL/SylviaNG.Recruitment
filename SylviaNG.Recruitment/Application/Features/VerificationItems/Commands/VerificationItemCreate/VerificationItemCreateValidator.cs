using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.VerificationItems.Commands.VerificationItemCreate
{
    public class VerificationItemCreateValidator : AbstractValidator<VerificationItemCreateCommand>
    {
        public VerificationItemCreateValidator()
        {
            RuleFor(x => x.Request.VerificationWorkflowId).GreaterThan(0).WithMessage("VerificationWorkflowId is required.");
        }
    }
}
