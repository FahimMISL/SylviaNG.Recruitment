using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderCreate
{
    public class GenderCreateValidator : AbstractValidator<GenderCreateCommand>
    {
        public GenderCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
