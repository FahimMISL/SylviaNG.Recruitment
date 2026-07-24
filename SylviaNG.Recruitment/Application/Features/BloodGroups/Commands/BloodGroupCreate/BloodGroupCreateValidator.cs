using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupCreate
{
    public class BloodGroupCreateValidator : AbstractValidator<BloodGroupCreateCommand>
    {
        public BloodGroupCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
