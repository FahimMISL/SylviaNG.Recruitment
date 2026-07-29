using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionCreate
{
    public class ReligionCreateValidator : AbstractValidator<ReligionCreateCommand>
    {
        public ReligionCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
