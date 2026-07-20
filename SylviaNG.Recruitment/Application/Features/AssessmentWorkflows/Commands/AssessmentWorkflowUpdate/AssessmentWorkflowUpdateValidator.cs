using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Commands.AssessmentWorkflowUpdate
{
    public class AssessmentWorkflowUpdateValidator : AbstractValidator<AssessmentWorkflowUpdateCommand>
    {
        public AssessmentWorkflowUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Workflow name is required.")
                .MaximumLength(200).WithMessage("Workflow name must not exceed 200 characters.");

            RuleFor(x => x.Request.Stages)
                .NotEmpty().WithMessage("A workflow must have at least one stage.");

            RuleForEach(x => x.Request.Stages).ChildRules(stage =>
            {
                stage.RuleFor(s => s.StageType)
                    .IsInEnum().WithMessage("Stage type is invalid.");

                stage.RuleFor(s => s.MaxMarks)
                    .GreaterThan(0).WithMessage("Max marks must be greater than 0.");

                stage.RuleFor(s => s.PassMarks)
                    .GreaterThan(0).WithMessage("Pass marks must be greater than 0.")
                    .LessThanOrEqualTo(s => s.MaxMarks).WithMessage("Pass marks must not exceed max marks.");

                stage.RuleFor(s => s.DurationMinutes)
                    .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");
            });
        }
    }
}
