using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Scorecards.Commands.ScorecardUpdate
{
    public class ScorecardUpdateValidator : AbstractValidator<ScorecardUpdateCommand>
    {
        public ScorecardUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Request.Criteria)
                .NotEmpty().WithMessage("At least one criterion is required.");

            RuleForEach(x => x.Request.Criteria).ChildRules(criterion =>
            {
                criterion.RuleFor(c => c.Name)
                    .NotEmpty().WithMessage("Criterion name is required.")
                    .MaximumLength(200).WithMessage("Criterion name must not exceed 200 characters.");

                criterion.RuleFor(c => c.Weight)
                    .GreaterThan(0).WithMessage("Weight must be greater than zero.");

                criterion.RuleFor(c => c.MaxScore)
                    .GreaterThan(0).WithMessage("Max score must be greater than zero.");
            });
        }
    }
}
