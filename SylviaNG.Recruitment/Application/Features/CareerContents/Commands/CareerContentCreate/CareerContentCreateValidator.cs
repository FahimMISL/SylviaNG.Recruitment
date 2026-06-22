using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.CareerContents.Commands.CareerContentCreate
{
    public class CareerContentCreateValidator : AbstractValidator<CareerContentCreateCommand>
    {
        public CareerContentCreateValidator()
        {
            RuleFor(x => x.Request.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(300).WithMessage("Title must not exceed 300 characters.");
        }
    }
}
