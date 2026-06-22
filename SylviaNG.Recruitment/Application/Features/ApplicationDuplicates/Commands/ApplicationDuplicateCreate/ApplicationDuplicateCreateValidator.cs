using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateCreate
{
    public class ApplicationDuplicateCreateValidator : AbstractValidator<ApplicationDuplicateCreateCommand>
    {
        public ApplicationDuplicateCreateValidator()
        {
            RuleFor(x => x.Request.PrimaryApplicationId)
                .GreaterThan(0).WithMessage("PrimaryApplicationId is required.");

            RuleFor(x => x.Request.MatchField)
                .NotEmpty().WithMessage("MatchField is required.")
                .MaximumLength(500).WithMessage("MatchField must not exceed 500 characters.");
        }
    }
}
