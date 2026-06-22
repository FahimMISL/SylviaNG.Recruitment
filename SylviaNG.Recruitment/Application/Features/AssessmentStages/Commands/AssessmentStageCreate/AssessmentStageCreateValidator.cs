using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageCreate
{
    public class AssessmentStageCreateValidator : AbstractValidator<AssessmentStageCreateCommand>
    {
        public AssessmentStageCreateValidator()
        {
            RuleFor(x => x.Request.AssessmentWorkflowId).GreaterThan(0).WithMessage("AssessmentWorkflowId is required.");
        }
    }
}
