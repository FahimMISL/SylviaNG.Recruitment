using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusCreate
{
    public class MaritalStatusCreateValidator : AbstractValidator<MaritalStatusCreateCommand>
    {
        public MaritalStatusCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
