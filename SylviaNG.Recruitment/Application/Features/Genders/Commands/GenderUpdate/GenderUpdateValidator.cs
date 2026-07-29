using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Genders.Commands.GenderUpdate
{
    public class GenderUpdateValidator : AbstractValidator<GenderUpdateCommand>
    {
        public GenderUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
