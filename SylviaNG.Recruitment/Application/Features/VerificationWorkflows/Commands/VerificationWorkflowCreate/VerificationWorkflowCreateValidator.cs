using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Commands.VerificationWorkflowCreate
{
    public class VerificationWorkflowCreateValidator : AbstractValidator<VerificationWorkflowCreateCommand>
    {
        public VerificationWorkflowCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
