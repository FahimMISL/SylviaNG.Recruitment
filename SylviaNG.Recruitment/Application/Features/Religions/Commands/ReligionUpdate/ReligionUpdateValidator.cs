using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Religions.Commands.ReligionUpdate
{
    public class ReligionUpdateValidator : AbstractValidator<ReligionUpdateCommand>
    {
        public ReligionUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
