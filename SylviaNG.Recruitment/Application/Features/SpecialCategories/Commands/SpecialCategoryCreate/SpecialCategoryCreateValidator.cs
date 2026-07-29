using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryCreate
{
    public class SpecialCategoryCreateValidator : AbstractValidator<SpecialCategoryCreateCommand>
    {
        public SpecialCategoryCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Special category name is required.")
                .MaximumLength(100).WithMessage("Special category name must not exceed 100 characters.");
        }
    }
}
