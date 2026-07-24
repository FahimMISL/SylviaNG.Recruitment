using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelines.Commands.HiringPipelineUpdate
{
    public class HiringPipelineUpdateValidator : AbstractValidator<HiringPipelineUpdateCommand>
    {
        public HiringPipelineUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Pipeline name is required.")
                .MaximumLength(200).WithMessage("Pipeline name must not exceed 200 characters.");

            RuleFor(x => x.Request.Stages)
                .NotEmpty().WithMessage("A pipeline must have at least one stage.");

            RuleForEach(x => x.Request.Stages).ChildRules(stage =>
            {
                stage.RuleFor(s => s.Name)
                    .NotEmpty().WithMessage("Stage name is required.")
                    .MaximumLength(200).WithMessage("Stage name must not exceed 200 characters.");

                stage.RuleFor(s => s.StageType)
                    .NotEmpty().WithMessage("Stage type is required.")
                    .MaximumLength(100).WithMessage("Stage type must not exceed 100 characters.");

                stage.RuleFor(s => s.SlaDays)
                    .GreaterThanOrEqualTo(0).When(s => s.SlaDays.HasValue)
                    .WithMessage("SLA days must be greater than or equal to 0.");

                stage.RuleFor(s => s.EstimatedDurationMinutes)
                    .GreaterThan(0).When(s => s.EstimatedDurationMinutes.HasValue)
                    .WithMessage("Estimated duration must be greater than 0 minutes.");

                stage.RuleFor(s => s.PassMarks)
                    .LessThanOrEqualTo(s => s.MaxMarks)
                    .When(s => s.MaxMarks.HasValue && s.PassMarks.HasValue)
                    .WithMessage("Pass marks must not exceed max marks.");
            });
        }
    }
}
