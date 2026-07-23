using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.MaritalStatuses.Commands.MaritalStatusUpdate
{
    public class MaritalStatusUpdateValidator : AbstractValidator<MaritalStatusUpdateCommand>
    {
        public MaritalStatusUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
