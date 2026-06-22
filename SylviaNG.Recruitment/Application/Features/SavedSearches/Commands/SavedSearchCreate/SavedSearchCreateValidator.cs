using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchCreate
{
    public class SavedSearchCreateValidator : AbstractValidator<SavedSearchCreateCommand>
    {
        public SavedSearchCreateValidator()
        {
            RuleFor(x => x.Request.CreatedByUserId)
                .GreaterThan(0).WithMessage("CreatedByUserId is required.");

            RuleFor(x => x.Request.SearchName)
                .NotEmpty().WithMessage("SearchName is required.")
                .MaximumLength(500).WithMessage("SearchName must not exceed 500 characters.");
        }
    }
}
