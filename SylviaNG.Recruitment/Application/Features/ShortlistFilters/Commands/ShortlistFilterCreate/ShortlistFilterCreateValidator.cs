using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate
{
    public class ShortlistFilterCreateValidator : AbstractValidator<ShortlistFilterCreateCommand>
    {
        public ShortlistFilterCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId)
                .GreaterThan(0).WithMessage("RequisitionId is required.");

            RuleFor(x => x.Request.FilterName)
                .NotEmpty().WithMessage("FilterName is required.")
                .MaximumLength(500).WithMessage("FilterName must not exceed 500 characters.");
        }
    }
}
