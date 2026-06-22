using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestCreate
{
    public class MedicalTestCreateValidator : AbstractValidator<MedicalTestCreateCommand>
    {
        public MedicalTestCreateValidator()
        {
            RuleFor(x => x.Request.VerificationWorkflowId)
                .GreaterThan(0).WithMessage("VerificationWorkflowId is required.");
        }
    }
}
