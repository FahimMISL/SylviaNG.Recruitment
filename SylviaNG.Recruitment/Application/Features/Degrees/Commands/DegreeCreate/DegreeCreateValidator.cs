using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeCreate
{
    public class DegreeCreateValidator : AbstractValidator<DegreeCreateCommand>
    {
        public DegreeCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Degree name is required.")
                .MaximumLength(50).WithMessage("Degree name must not exceed 50 characters.");

            RuleFor(x => x.Request.FullName)
                .NotEmpty().WithMessage("Degree full name is required.")
                .MaximumLength(200).WithMessage("Degree full name must not exceed 200 characters.");

            RuleFor(x => x.Request.Position)
                .GreaterThan(0).WithMessage("Position must be a positive integer (equivalence group number).");
        }
    }
}
