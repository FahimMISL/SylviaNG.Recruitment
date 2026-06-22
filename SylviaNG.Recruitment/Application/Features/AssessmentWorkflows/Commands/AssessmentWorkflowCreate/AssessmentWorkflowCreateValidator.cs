using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowCreate
{
    public class AssessmentWorkflowCreateValidator : AbstractValidator<AssessmentWorkflowCreateCommand>
    {
        public AssessmentWorkflowCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId)
                .GreaterThan(0).WithMessage("RequisitionId is required.");

            RuleFor(x => x.Request.WorkflowName)
                .NotEmpty().WithMessage("WorkflowName is required.")
                .MaximumLength(500).WithMessage("WorkflowName must not exceed 500 characters.");
        }
    }
}
