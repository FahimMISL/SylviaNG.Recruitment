using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupUpdate
{
    public class BloodGroupUpdateValidator : AbstractValidator<BloodGroupUpdateCommand>
    {
        public BloodGroupUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
